"""Rebuild Shadowgun's shaders from the Unity 3.5 GLES2 programs that AssetRipper exports.

Unity 3.5 shipped every shader on Android as hlsl2glsl output ("!!GLES" sub-programs, one per keyword variant).
This turns each Program "vp"/"fp" pair back into a CGPROGRAM block that Unity 2018.2 (PSP2) compiles:

  * the GLSL is parsed (it is very regular: tmpvar_N temporaries, xlv_ varyings, one main() per stage) and
    re-emitted as HLSL, so the maths stays exactly the shipped maths;
  * GLSL matrices are column-major. Matrices are kept "transposed" (an HLSL row = a GLSL column), so M[i] keeps
    its meaning and M * v becomes mul(v, M); Unity's own matrices go through transpose()/mul(U, v);
  * Unity 3.5 built-ins that no longer exist are emulated: _World2Object (inverse world matrix WITHOUT scale)
    and unity_Scale (w = 1 / object scale). Lightmaps keep the shader's own "2.0 * sample" (dLDR);
  * keyword variants: LIGHTMAP_OFF/ON -> multi_compile _ LIGHTMAP_ON, VERTEXLIGHT_ON kept, realtime shadow
    variants dropped (the game draws blob shadows; PSP2 has no screen-space shadows);
  * GLSL precision: highp -> float, mediump and lowp -> half. The shipped code relies on Android's lowp being fp16
    (e.g. Blinking emissive builds a seed as dot(color, color) * 40 in lowp), so lowp must not become a +-2 fixed.

Usage:  python sg_glsl2hlsl.py <decomp .shader> <out .shader>      (or --all <decomp Assets dir> <out dir>)
"""
import os
import re
import sys

PREC = {"highp": "float", "mediump": "half", "lowp": "half", None: "float"}
TYPES = {"float", "int", "bool", "void", "sampler2D", "samplerCube",
         "vec2", "vec3", "vec4", "ivec2", "ivec3", "ivec4", "bvec2", "bvec3", "bvec4", "mat2", "mat3", "mat4"}
QUALS = {"const", "highp", "mediump", "lowp"}

# Unity 3.5 uniforms that UnityCG.cginc / UnityShaderVariables.cginc already declare (or that are emulated).
BUILTIN_UNIFORMS = {
    "glstate_matrix_mvp", "glstate_matrix_modelview0", "glstate_matrix_invtrans_modelview0",
    "glstate_matrix_projection", "glstate_matrix_transpose_modelview0", "glstate_lightmodel_ambient",
    "_Object2World", "_World2Object", "unity_Scale",
    "_Time", "_SinTime", "_CosTime", "unity_DeltaTime", "_WorldSpaceCameraPos", "_ProjectionParams",
    "_ScreenParams", "_ZBufferParams", "_WorldSpaceLightPos0",
    "unity_4LightPosX0", "unity_4LightPosY0", "unity_4LightPosZ0", "unity_4LightAtten0",
    "unity_LightColor", "unity_LightPosition", "unity_LightAtten", "unity_SpotDirection",
    "unity_SHAr", "unity_SHAg", "unity_SHAb", "unity_SHBr", "unity_SHBg", "unity_SHBb", "unity_SHC",
    "unity_LightmapST", "unity_Lightmap", "unity_LightmapInd",
}
# Unity matrices, Unity convention (GLSL M * v == HLSL mul(M, v)).
UNITY_MATRIX = {
    "glstate_matrix_mvp": "UNITY_MATRIX_MVP",
    "glstate_matrix_modelview0": "UNITY_MATRIX_MV",
    "glstate_matrix_invtrans_modelview0": "UNITY_MATRIX_IT_MV",
    "glstate_matrix_transpose_modelview0": "UNITY_MATRIX_T_MV",
    "glstate_matrix_projection": "UNITY_MATRIX_P",
    "_Object2World": "unity_ObjectToWorld",
    "_World2Object": "sg_World2Object()",
}
OTHER_BUILTINS = {"unity_Scale": "sg_unity_Scale()", "glstate_lightmodel_ambient": "UNITY_LIGHTMODEL_AMBIENT"}
ATTRIBUTES = {
    "_glesVertex": ("vertex", "float4", "POSITION"), "_glesColor": ("color", "float4", "COLOR"),
    "_glesNormal": ("normal", "float3", "NORMAL"), "_glesTANGENT": ("tangent", "float4", "TANGENT"),
    "_glesMultiTexCoord0": ("texcoord", "float4", "TEXCOORD0"),
    "_glesMultiTexCoord1": ("texcoord1", "float4", "TEXCOORD1"),
}
HLSL_RESERVED = {
    "v", "o", "i", "sampler", "texture", "line", "point", "triangle", "lineadj", "triangleadj", "matrix",
    "vector", "string", "pass", "technique", "compile", "register", "packoffset", "in", "out", "inout",
    "uniform", "extern", "shared", "static", "volatile", "precise", "nointerpolation", "linear", "centroid",
    "noperspective", "sample", "half", "fixed", "double", "snorm", "unorm", "row_major", "column_major",
    "interface", "class", "namespace", "typedef", "export", "asm", "tbuffer", "cbuffer", "struct",
    "pixelshader", "vertexshader", "stateblock", "dword", "sampler1D", "sampler3D", "samplerCUBE",
}
GENTYPE_FUNCS = {"normalize", "abs", "sign", "floor", "ceil", "fract", "sin", "cos", "tan", "asin", "acos",
                 "atan", "exp", "log", "exp2", "log2", "sqrt", "inversesqrt", "radians", "degrees", "pow", "mod",
                 "min", "max", "clamp", "mix", "step", "smoothstep", "reflect", "refract", "faceforward"}
FUNC_RENAME = {"fract": "frac", "mix": "lerp", "inversesqrt": "rsqrt", "mod": "sg_mod", "dFdx": "ddx",
               "dFdy": "ddy", "textureCube": "texCUBE"}
CMP_FUNCS = {"lessThan": "<", "lessThanEqual": "<=", "greaterThan": ">", "greaterThanEqual": ">=",
             "equal": "==", "notEqual": "!="}

HELPERS = {
    "sg_mod": """// GLSL mod(): floor-based (HLSL fmod truncates)
inline float sg_mod (float x, float y) { return x - y * floor(x / y); }
inline float2 sg_mod (float2 x, float2 y) { return x - y * floor(x / y); }
inline float3 sg_mod (float3 x, float3 y) { return x - y * floor(x / y); }
inline float4 sg_mod (float4 x, float4 y) { return x - y * floor(x / y); }
""",
    "sg_ObjScale": """// uniform scale of the object (Unity 3.5 kept it apart from _World2Object)
inline float sg_ObjScale () { return length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)); }
""",
    "sg_World2Object": """// Unity 3.5 _World2Object = inverse world matrix with the scale removed (code multiplies by unity_Scale.w)
inline float4x4 sg_World2Object () { float s = sg_ObjScale(); float4x4 m = unity_WorldToObject; m[0] *= s; m[1] *= s; m[2] *= s; return m; }
""",
    "sg_unity_Scale": """// Unity 3.5 unity_Scale: w = 1 / object scale
inline float4 sg_unity_Scale () { float s = sg_ObjScale(); return float4(s, s, s, 1.0 / s); }
""",
}
HELPERS["sg_FromGLClip"] = """// The game writes GL clip space itself here (no matrix): z in [-w, w], never flipped. Follow this platform's
// depth range (reversed Z on D3D11; PSP2 is GL-like) and Unity's projection flip when rendering into a texture.
inline float4 sg_FromGLClip (float4 p)
{
#if defined(UNITY_REVERSED_Z)
  p.z = (p.w - p.z) * 0.5;
#endif
  p.y *= _ProjectionParams.x;
  return p;
}
"""
HELPER_DEPS = {"sg_World2Object": ["sg_ObjScale"], "sg_unity_Scale": ["sg_ObjScale"]}


class ConvertError(Exception):
    pass


# ----------------------------------------------------------------------------------------------- tokenizer / parser
TOKEN_RE = re.compile(r"\s*(?:(\d+\.\d*(?:[eE][-+]?\d+)?|\.\d+(?:[eE][-+]?\d+)?|\d+(?:[eE][-+]?\d+)?)|([A-Za-z_]\w*)|"
                      r"(\+\+|--|\+=|-=|\*=|/=|==|!=|<=|>=|&&|\|\||\^\^|[-+*/<>=!?:;,.()\[\]{}]))")


def tokenize(src):
    toks, pos = [], 0
    src = src.rstrip()
    while pos < len(src):
        m = TOKEN_RE.match(src, pos)
        if not m or m.end() == pos:
            if src[pos:].strip() == "":
                break
            raise ConvertError("cannot tokenize near: %r" % src[pos:pos + 40])
        if m.group(1):
            toks.append(("num", m.group(1)))
        elif m.group(2):
            toks.append(("id", m.group(2)))
        elif m.group(3):
            toks.append(("op", m.group(3)))
        pos = m.end()
    return toks


class Parser:
    def __init__(self, toks):
        self.t, self.i = toks, 0

    def peek(self, k=0):
        j = self.i + k
        return self.t[j] if j < len(self.t) else (None, None)

    def next(self):
        tok = self.peek()
        self.i += 1
        return tok

    def accept(self, v):
        if self.peek()[1] == v:
            self.i += 1
            return True
        return False

    def expect(self, v):
        tok = self.next()
        if tok[1] != v:
            raise ConvertError("expected %r got %r (token %d)" % (v, tok, self.i))
        return tok

    # --- top level
    def translation_unit(self):
        decls, main = [], None
        while self.peek()[0] is not None:
            if self.peek()[1] == "precision":
                while self.next()[1] != ";":
                    pass
                continue
            if self.peek()[1] in ("uniform", "varying", "attribute"):
                storage = self.next()[1]
                prec = self.next()[1] if self.peek()[1] in QUALS else None
                typ = self.next()[1]
                name = self.next()[1]
                arr = None
                if self.accept("["):
                    arr = self.next()[1]
                    self.expect("]")
                self.expect(";")
                decls.append((storage, prec, typ, name, arr))
                continue
            if self.peek()[1] == "void" and self.peek(1)[1] == "main":
                self.next(); self.next(); self.expect("("); self.expect(")")
                main = self.block()
                continue
            if self.peek()[1] in TYPES or self.peek()[1] in QUALS:
                # global non-uniform variable (static in HLSL)
                d = self.declaration()
                decls.append(("global", d))
                continue
            raise ConvertError("unexpected top-level token %r" % (self.peek(),))
        return decls, main

    def block(self):
        self.expect("{")
        stmts = []
        while not self.accept("}"):
            stmts.append(self.statement())
        return ("block", stmts)

    def is_decl_start(self):
        k = self.peek()[1]
        if k in QUALS:
            return True
        return k in TYPES and self.peek(1)[0] == "id"

    def declaration(self):
        const = False
        prec = None
        while self.peek()[1] in QUALS:
            q = self.next()[1]
            if q == "const":
                const = True
            else:
                prec = q
        typ = self.next()[1]
        items = []
        while True:
            name = self.next()[1]
            arr = None
            if self.accept("["):
                arr = self.next()[1]
                self.expect("]")
            init = None
            if self.accept("="):
                init = self.assignment()
            items.append((name, arr, init))
            if not self.accept(","):
                break
        self.expect(";")
        return ("decl", const, prec, typ, items)

    def statement(self):
        k = self.peek()[1]
        if k == "{":
            return self.block()
        if k == ";":
            self.next()
            return ("empty",)
        if k == "if":
            self.next(); self.expect("(")
            c = self.expression()
            self.expect(")")
            s = self.statement()
            e = None
            if self.accept("else"):
                e = self.statement()
            return ("if", c, s, e)
        if k == "for":
            self.next(); self.expect("(")
            init = self.declaration() if self.is_decl_start() else ("expr", self.expression()) if not self.accept(";") else None
            if init and init[0] == "expr":
                self.expect(";")
            cond = None if self.peek()[1] == ";" else self.expression()
            self.expect(";")
            step = None if self.peek()[1] == ")" else self.expression()
            self.expect(")")
            return ("for", init, cond, step, self.statement())
        if k == "return":
            self.next()
            e = None if self.peek()[1] == ";" else self.expression()
            self.expect(";")
            return ("ret", e)
        if k == "discard":
            self.next(); self.expect(";")
            return ("discard",)
        if self.is_decl_start():
            return self.declaration()
        e = self.expression()
        self.expect(";")
        return ("expr", e)

    # --- expressions
    def expression(self):
        e = self.assignment()
        while self.accept(","):
            e = ("comma", e, self.assignment())
        return e

    def assignment(self):
        lhs = self.ternary()
        if self.peek()[1] in ("=", "+=", "-=", "*=", "/="):
            op = self.next()[1]
            return ("asg", op, lhs, self.assignment())
        return lhs

    def ternary(self):
        c = self.binary(0)
        if self.accept("?"):
            a = self.expression()
            self.expect(":")
            b = self.assignment()
            return ("tern", c, a, b)
        return c

    LEVELS = [("||",), ("^^",), ("&&",), ("==", "!="), ("<", ">", "<=", ">="), ("+", "-"), ("*", "/")]

    def binary(self, level):
        if level == len(self.LEVELS):
            return self.unary()
        e = self.binary(level + 1)
        while self.peek()[0] == "op" and self.peek()[1] in self.LEVELS[level]:
            op = self.next()[1]
            e = ("bin", op, e, self.binary(level + 1))
        return e

    def unary(self):
        if self.peek()[0] == "op" and self.peek()[1] in ("-", "+", "!", "++", "--"):
            op = self.next()[1]
            return ("un", op, self.unary())
        return self.postfix()

    def postfix(self):
        e = self.primary()
        while True:
            k = self.peek()[1]
            if k == "(" and e[0] == "id":
                self.next()
                args = []
                if not self.accept(")"):
                    while True:
                        args.append(self.assignment())
                        if self.accept(")"):
                            break
                        self.expect(",")
                e = ("call", e[1], args)
            elif k == "[":
                self.next()
                idx = self.expression()
                self.expect("]")
                e = ("idx", e, idx)
            elif k == ".":
                self.next()
                e = ("mem", e, self.next()[1])
            elif k in ("++", "--"):
                self.next()
                e = ("post", k, e)
            else:
                return e

    def primary(self):
        kind, v = self.next()
        if kind == "num":
            return ("num", v)
        if kind == "id":
            return ("id", v)
        if v == "(":
            e = self.expression()
            self.expect(")")
            return ("paren", e)
        raise ConvertError("unexpected token %r" % v)


# ----------------------------------------------------------------------------------------------- types
def vec_size(t):
    m = re.fullmatch(r"[ib]?vec([234])", t or "")
    return int(m.group(1)) if m else (1 if t in ("float", "int", "bool") else 0)


def mat_size(t):
    m = re.fullmatch(r"mat([234])", t or "")
    return int(m.group(1)) if m else 0


class Stage:
    """Translation context of one GLSL stage."""

    def __init__(self, kind, env, varyings, used):
        self.kind = kind          # "vertex" / "fragment"
        self.env = env            # name -> (type, array)
        self.varyings = varyings  # names
        self.used = used          # helper names used
        self.renames = {}
        self.warnings = []

    def type_of(self, e):
        k = e[0]
        if k == "paren":
            return self.type_of(e[1])
        if k == "num":
            return "float" if re.search(r"[.eE]", e[1]) else "int"
        if k == "id":
            t = self.env.get(e[1])
            return t[0] if t else None
        if k == "call":
            n, args = e[1], e[2]
            if n in TYPES:
                return n
            if n in ("texture2D", "texture2DProj", "textureCube", "texture2DLod", "texture2DProjLod", "textureCubeLod"):
                return "vec4"
            if n in ("dot", "length", "distance"):
                return "float"
            if n == "cross":
                return "vec3"
            if n in CMP_FUNCS:
                return "bvec%d" % vec_size(self.type_of(args[0]))
            if n in ("any", "all"):
                return "bool"
            if n == "not":
                return self.type_of(args[0])
            if n in GENTYPE_FUNCS or n in ("dFdx", "dFdy", "fwidth"):
                best = None
                for a in args:
                    t = self.type_of(a)
                    if best is None or vec_size(t) > vec_size(best):
                        best = t
                return best
            return None
        if k == "idx":
            bt = self.type_of(e[1])
            if mat_size(bt):
                return "vec%d" % mat_size(bt)
            if e[1][0] == "id" and self.env.get(e[1][1], (None, None))[1]:
                return bt  # array element
            if vec_size(bt) > 1:
                return "float"
            return bt
        if k == "mem":
            n = len(e[2])
            return "float" if n == 1 else "vec%d" % n
        if k == "un":
            return "bool" if e[1] == "!" else self.type_of(e[2])
        if k == "post":
            return self.type_of(e[2])
        if k == "bin":
            op, l, r = e[1], e[2], e[3]
            if op in ("<", ">", "<=", ">=", "==", "!=", "&&", "||", "^^"):
                return "bool"
            lt, rt = self.type_of(l), self.type_of(r)
            if op == "*" and mat_size(lt) and mat_size(rt):
                return lt
            if op == "*" and mat_size(lt):
                return "vec%d" % mat_size(lt) if vec_size(rt) > 1 or rt is None else lt
            if op == "*" and mat_size(rt):
                return "vec%d" % mat_size(rt) if vec_size(lt) > 1 or lt is None else rt
            if mat_size(lt) or mat_size(rt):
                return lt if mat_size(lt) else rt
            return lt if vec_size(lt) >= vec_size(rt) else rt
        if k == "asg":
            return self.type_of(e[2])
        if k == "tern":
            return self.type_of(e[2])
        if k == "comma":
            return self.type_of(e[2])
        return None

    def is_mat(self, e):
        return mat_size(self.type_of(e)) > 0

    # ------------------------------------------------------------------------------------------- emission
    def name(self, n):
        if n in self.renames:
            return self.renames[n]
        if n in ATTRIBUTES:
            if self.kind != "vertex":
                raise ConvertError("attribute in fragment stage")
            return "v." + ATTRIBUTES[n][0]
        if n in self.varyings:
            return ("o." if self.kind == "vertex" else "i.") + n
        if n == "gl_Position":
            return "o.pos"
        if n == "gl_FragColor":
            return "sg_FragColor"
        if n in OTHER_BUILTINS:
            r = OTHER_BUILTINS[n]
            if r.startswith("sg_"):
                self.used.add(r[:-2])
            if n == "glstate_lightmodel_ambient":
                self.warnings.append("glstate_lightmodel_ambient mapped to UNITY_LIGHTMODEL_AMBIENT (scale may differ)")
            return r
        if n in HLSL_RESERVED:
            self.renames[n] = n + "_g"
            return n + "_g"
        return n

    def mat_T(self, e):
        """Matrix-valued expression -> ('tr', X) meaning transpose(X) with X in Unity convention, or ('raw', s)."""
        k = e[0]
        if k == "paren":
            return self.mat_T(e[1])
        if k == "id":
            if e[1] in UNITY_MATRIX:
                u = UNITY_MATRIX[e[1]]
                if u.startswith("sg_"):
                    self.used.add(u[:-2])
                return ("tr", u)
            t = self.env.get(e[1])
            if t and t[2] == "uniform":
                return ("tr", self.name(e[1]))  # material/script matrices: Unity convention as well
            return ("raw", self.name(e[1]))
        if k == "call" and mat_size(e[1]):
            n = mat_size(e[1])
            ht = "float%dx%d" % (n, n)
            args = e[2]
            if len(args) == 1 and self.is_mat(args[0]):
                inner = self.mat_T(args[0])
                if inner[0] == "tr":
                    return ("tr", "((%s)%s)" % (ht, inner[1]))
                return ("raw", "((%s)%s)" % (ht, inner[1]))
            if len(args) == 1:
                s = self.emit(args[0])
                vals = [s if r == c else "0.0" for c in range(n) for r in range(n)]
                return ("raw", "%s(%s)" % (ht, ", ".join(vals)))
            # columns (or column-major scalars) -> rows of the transposed matrix: same order
            return ("raw", "%s(%s)" % (ht, ", ".join(self.emit(a) for a in args)))
        if k == "bin" and e[1] == "*" and self.is_mat(e[2]) and self.is_mat(e[3]):
            return ("raw", "mul(%s, %s)" % (self.mat_str(e[3]), self.mat_str(e[2])))
        if k == "bin":
            return ("raw", "(%s %s %s)" % (self.emit(e[2]), e[1], self.emit(e[3])))
        if k == "un":
            return ("raw", "(%s%s)" % (e[1], self.mat_str(e[2])))
        if k == "asg":
            return ("raw", self.emit(e))
        raise ConvertError("unsupported matrix expression %r" % (e,))

    def mat_str(self, e):
        k, s = self.mat_T(e)
        return "transpose(%s)" % s if k == "tr" else s

    def emit(self, e):
        k = e[0]
        if k == "paren":
            inner = self.emit(e[1])
            return inner if inner.startswith("(") and inner.endswith(")") and e[1][0] in ("bin", "paren") else "(%s)" % inner
        if k == "num":
            return e[1]
        if k == "id":
            if self.is_mat(e):
                return self.mat_str(e)
            return self.name(e[1])
        if k == "call":
            return self.emit_call(e[1], e[2])
        if k == "idx":
            if e[1][0] == "id" and e[1][1] == "gl_FragData":
                return "sg_FragColor"
            if self.is_mat(e[1]):
                return "%s[%s]" % (self.mat_str(e[1]), self.emit(e[2]))
            return "%s[%s]" % (self.emit(e[1]), self.emit(e[2]))
        if k == "mem":
            f = e[2]
            if re.fullmatch(r"[stpq]+", f):
                f = f.translate(str.maketrans("stpq", "xyzw"))
            return "%s.%s" % (self.emit(e[1]), f)
        if k == "un":
            if e[1] == "+":
                return self.emit(e[2])
            return "%s%s" % (e[1], self.emit_operand(e[2]))
        if k == "post":
            return "%s%s" % (self.emit(e[2]), e[1])
        if k == "bin":
            op, l, r = e[1], e[2], e[3]
            if op == "*":
                lm, rm = self.is_mat(l), self.is_mat(r)
                lt, rt = self.type_of(l), self.type_of(r)
                if lm and rm:
                    return self.mat_str(e)
                if lm and vec_size(rt) > 1:
                    # GLSL M * v
                    if l[0] == "id" and l[1] == "glstate_matrix_mvp":
                        if r[0] == "id" and r[1] == "_glesVertex":
                            return "UnityObjectToClipPos(v.vertex)"
                        # exact for any w; Unity's import upgrader would turn mul(UNITY_MATRIX_MVP, x) into
                        # UnityObjectToClipPos(x), which assumes w == 1, and rewrite the file
                        return "mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, %s))" % self.emit(r)
                    kind, s = self.mat_T(l)
                    return "mul(%s, %s)" % (s, self.emit(r)) if kind == "tr" else "mul(%s, %s)" % (self.emit(r), s)
                if rm and vec_size(lt) > 1:
                    # GLSL v * M
                    kind, s = self.mat_T(r)
                    return "mul(%s, %s)" % (self.emit(l), s) if kind == "tr" else "mul(%s, %s)" % (s, self.emit(l))
                if lm or rm:
                    return "(%s * %s)" % (self.emit(l), self.emit(r))  # matrix * scalar
            if op == "^^":
                return "(bool(%s) != bool(%s))" % (self.emit(l), self.emit(r))
            return "(%s %s %s)" % (self.emit(l), op, self.emit(r))
        if k == "asg":
            op, l, r = e[1], e[2], e[3]
            if op == "*=" and self.is_mat(r) and not self.is_mat(l):
                return "%s = %s" % (self.emit(l), self.emit(("bin", "*", l, r)))
            return "%s %s %s" % (self.emit(l), op, self.emit(r))
        if k == "tern":
            return "(%s ? %s : %s)" % (self.emit(e[1]), self.emit(e[2]), self.emit(e[3]))
        if k == "comma":
            return "%s, %s" % (self.emit(e[1]), self.emit(e[2]))
        raise ConvertError("cannot emit %r" % (e,))

    def emit_operand(self, e):
        s = self.emit(e)
        if e[0] in ("id", "num", "call", "idx", "mem", "paren") or (s.startswith("(") and s.endswith(")")):
            return s
        return "(%s)" % s

    def emit_call(self, n, args):
        a = [self.emit(x) for x in args]
        if n in TYPES:
            if mat_size(n):
                return self.mat_str(("call", n, args))
            ht = {"float": "float", "int": "int", "bool": "bool"}.get(n)
            if not ht:
                m = re.fullmatch(r"([ib]?)vec([234])", n)
                ht = {"": "float", "i": "int", "b": "bool"}[m.group(1)] + m.group(2)
            if len(a) == 1:
                return "((%s)%s)" % (ht, self.emit_operand(args[0]))
            return "%s(%s)" % (ht, ", ".join(a))
        if n == "texture2D":
            if args[0][0] == "id" and args[0][1] == "unity_Lightmap":
                return "UNITY_SAMPLE_TEX2D(unity_Lightmap, %s)" % a[1]
            if len(a) == 3:
                return "tex2Dbias(%s, float4(%s, 0.0, %s))" % (a[0], a[1], a[2])
            return "tex2D(%s, %s)" % (a[0], a[1])
        if n == "texture2DProj":
            if vec_size(self.type_of(args[1])) == 3:
                return "tex2D(%s, (%s).xy / (%s).z)" % (a[0], a[1], a[1])
            return "tex2Dproj(%s, %s)" % (a[0], a[1])
        if n == "texture2DLod":
            return "tex2Dlod(%s, float4(%s, 0.0, %s))" % (a[0], a[1], a[2])
        if n == "textureCubeLod":
            return "texCUBElod(%s, float4(%s, %s))" % (a[0], a[1], a[2])
        if n in CMP_FUNCS:
            return "(%s %s %s)" % (self.emit_operand(args[0]), CMP_FUNCS[n], self.emit_operand(args[1]))
        if n == "not":
            return "(!%s)" % self.emit_operand(args[0])
        if n == "atan" and len(a) == 2:
            return "atan2(%s, %s)" % (a[0], a[1])
        if n == "mod":
            self.used.add("sg_mod")
            # GLSL mod(genType, float): broadcast the scalar so the overload matches
            t0, t1 = self.type_of(args[0]), self.type_of(args[1])
            if vec_size(t0) > 1 and vec_size(t1) == 1:
                a[1] = "((float%d)%s)" % (vec_size(t0), self.emit_operand(args[1]))
            if vec_size(t1) > 1 and vec_size(t0) == 1:
                a[0] = "((float%d)%s)" % (vec_size(t1), self.emit_operand(args[0]))
        return "%s(%s)" % (FUNC_RENAME.get(n, n), ", ".join(a))

    def emit_stmt(self, s, ind, out):
        p = "  " * ind
        k = s[0]
        if k == "block":
            for x in s[1]:
                self.emit_stmt(x, ind, out)
        elif k == "empty":
            pass
        elif k == "decl":
            _, const, prec, typ, items = s
            for (n, arr, init) in items:
                self.env[n] = (typ, arr, "local")
                ht = hl_type(prec, typ)
                line = "%s%s%s %s%s" % (p, "const " if const else "", ht, self.name(n), "[%s]" % arr if arr else "")
                if init is not None:
                    line += " = " + self.emit(init)
                out.append(line + ";")
        elif k == "expr":
            out.append("%s%s;" % (p, self.emit(s[1])))
        elif k == "if":
            out.append("%sif (%s) {" % (p, strip_parens(self.emit(s[1]))))
            self.emit_stmt(s[2], ind + 1, out)
            if s[3] is not None:
                out.append("%s} else {" % p)
                self.emit_stmt(s[3], ind + 1, out)
            out.append("%s}" % p)
        elif k == "for":
            init = ""
            if s[1] is not None:
                tmp = []
                self.emit_stmt(s[1], 0, tmp)
                init = tmp[0].rstrip(";")
            out.append("%sfor (%s; %s; %s) {" % (p, init, self.emit(s[2]) if s[2] else "", self.emit(s[3]) if s[3] else ""))
            self.emit_stmt(s[4], ind + 1, out)
            out.append("%s}" % p)
        elif k == "ret":
            out.append("%sreturn%s;" % (p, (" " + self.emit(s[1])) if s[1] else ""))
        elif k == "discard":
            out.append("%sdiscard;" % p)
        else:
            raise ConvertError("statement %r" % (s,))


def strip_parens(s):
    if s.startswith("(") and s.endswith(")"):
        depth = 0
        for j, c in enumerate(s):
            depth += c == "("
            depth -= c == ")"
            if depth == 0 and j != len(s) - 1:
                return s
        return s[1:-1]
    return s


def hl_type(prec, t):
    base = PREC.get(prec, "float")
    m = re.fullmatch(r"vec([234])", t)
    if m:
        return base + m.group(1)
    m = re.fullmatch(r"mat([234])", t)
    if m:
        return "%s%sx%s" % (base, m.group(1), m.group(1))
    if t == "float":
        return base
    m = re.fullmatch(r"([ib])vec([234])", t)
    if m:
        return {"i": "int", "b": "bool"}[m.group(1)] + m.group(2)
    return {"int": "int", "bool": "bool", "void": "void", "sampler2D": "sampler2D", "samplerCube": "samplerCUBE"}[t]


# ----------------------------------------------------------------------------------------------- one GLES program
def split_stages(glsl):
    v = re.search(r"#ifdef VERTEX(.*?)#endif", glsl, re.S)
    f = re.search(r"#ifdef FRAGMENT(.*?)#endif", glsl, re.S)
    if not v or not f:
        raise ConvertError("no VERTEX/FRAGMENT section")
    head = glsl[:v.start()]
    return head, v.group(1), f.group(1)


def preprocess(src, defines):
    lines = []
    for line in src.split("\n"):
        s = line.strip()
        m = re.match(r"#define\s+(\w+)\s+(.*)$", s)
        if m:
            defines[m.group(1)] = m.group(2).strip()
            continue
        if s.startswith("#"):
            continue  # #extension, #line, ...
        lines.append(line)
    code = "\n".join(lines)
    for k, val in defines.items():
        if k.startswith("SHADER_API") or k == "tex2D":
            continue
        code = re.sub(r"\b%s\b" % re.escape(k), val, code)
    return code


def convert_program(glsl):
    """-> dict(uniforms={name: (prec, type, arr)}, varyings=[(prec, type, name)], attrs=set, vbody, fbody, used, warnings)"""
    head, vsrc, fsrc = split_stages(glsl)
    defines = {}
    preprocess(head, defines)
    stages = {}
    uniforms, varyings, attrs, used, warnings = {}, [], set(), set(), []
    var_names = []
    for kind, src in (("vertex", vsrc), ("fragment", fsrc)):
        code = preprocess(src, dict(defines))
        decls, main = Parser(tokenize(code)).translation_unit()
        if main is None:
            raise ConvertError("%s stage has no main()" % kind)
        env = {}
        globals_ = []
        for d in decls:
            if d[0] == "global":
                globals_.append(d[1])
                continue
            storage, prec, typ, name, arr = d
            env[name] = (typ, arr, storage)
            if storage == "uniform":
                if name in BUILTIN_UNIFORMS:
                    continue
                old = uniforms.get(name)
                rank = {"lowp": 0, "mediump": 1, "highp": 2, None: 2}
                if old is None or rank[prec] > rank[old[0]]:
                    uniforms[name] = (prec, typ, arr)
            elif storage == "varying":
                if name not in var_names:
                    var_names.append(name)
                    varyings.append((prec, typ, name))
            elif storage == "attribute":
                if name not in ATTRIBUTES:
                    raise ConvertError("unknown attribute %s" % name)
                attrs.add(name)
        st = Stage(kind, env, set(var_names), used)
        out = []
        raw_clip = kind == "vertex" and not re.search(r"\bglstate_matrix_(mvp|projection)\b", code)
        for g in globals_:
            # GLSL globals are per-invocation: static locals at the top of main
            st.emit_stmt(g, 1, out)
        st.emit_stmt(main, 1, out)
        if raw_clip:
            used.add("sg_FromGLClip")
            out.append("  o.pos = sg_FromGLClip(o.pos);")
            warnings.append("vertex program writes clip space directly: sg_FromGLClip applied")
        stages[kind] = out
        warnings += st.warnings
    return dict(uniforms=uniforms, varyings=varyings, attrs=attrs, vbody=stages["vertex"], fbody=stages["fragment"],
                used=used, warnings=warnings)


def emit_variant(prog, ind="    "):
    out = []
    out.append("struct sg_appdata {")
    for a in sorted(prog["attrs"], key=lambda n: list(ATTRIBUTES).index(n)):
        field, typ, sem = ATTRIBUTES[a]
        out.append("  %s %s : %s;" % (typ, field, sem))
    if "_glesVertex" not in prog["attrs"]:
        out.append("  float4 vertex : POSITION;")
    out.append("};")
    out.append("struct sg_v2f {")
    out.append("  float4 pos : SV_POSITION;")
    used_sem = set()
    pending = []
    for prec, typ, name in prog["varyings"]:
        m = re.fullmatch(r"xlv_TEXCOORD(\d+)", name)
        if m:
            used_sem.add(int(m.group(1)))
            out.append("  %s %s : TEXCOORD%s;" % (hl_type(prec, typ), name, m.group(1)))
        else:
            pending.append((prec, typ, name))
    nxt = 0
    color_sem = 0
    for prec, typ, name in pending:
        if re.fullmatch(r"xlv_COLOR\d?", name) and color_sem < 2:
            out.append("  %s %s : COLOR%d;" % (hl_type(prec, typ), name, color_sem))
            color_sem += 1
            continue
        while nxt in used_sem:
            nxt += 1
        used_sem.add(nxt)
        out.append("  %s %s : TEXCOORD%d;" % (hl_type(prec, typ), name, nxt))
    out.append("};")
    out.append("sg_v2f vert (sg_appdata v)")
    out.append("{")
    out.append("  sg_v2f o;")
    out.append("  UNITY_INITIALIZE_OUTPUT(sg_v2f, o);")
    out += prog["vbody"]
    out.append("  return o;")
    out.append("}")
    out.append("fixed4 frag (sg_v2f i) : SV_Target")
    out.append("{")
    out.append("  fixed4 sg_FragColor = 0;")
    out += prog["fbody"]
    out.append("  return sg_FragColor;")
    out.append("}")
    return [ind + l if l else l for l in out]


# ----------------------------------------------------------------------------------------------- ShaderLab
DROP_KEYWORDS = {"SHADOWS_SCREEN", "SHADOWS_NATIVE", "SHADOWS_DEPTH", "SHADOWS_CUBE", "SHADOWS_SOFT",
                 "DIRLIGHTMAP_ON", "POINT", "SPOT", "POINT_COOKIE", "DIRECTIONAL_COOKIE"}


def find_block_end(text, open_idx):
    """text[open_idx] == '{' -> index after the matching '}' (skips "..." strings)"""
    depth, i, in_str = 0, open_idx, False
    while i < len(text):
        c = text[i]
        if in_str:
            if c == '"':
                in_str = False
        elif c == '"':
            in_str = True
        elif c == "{":
            depth += 1
        elif c == "}":
            depth -= 1
            if depth == 0:
                return i + 1
        i += 1
    raise ConvertError("unbalanced ShaderLab block")


def parse_subprograms(block):
    subs = []
    for m in re.finditer(r'SubProgram\s+"gles\s*"\s*\{', block):
        end = find_block_end(block, m.end() - 1)
        body = block[m.end():end - 1]
        kw = re.search(r"Keywords\s*\{([^}]*)\}", body)
        keywords = set(re.findall(r'"(\w+)"', kw.group(1))) if kw else set()
        src = re.search(r'"(!!GLES.*?)"\s*$', body, re.S)
        subs.append((keywords, src.group(1) if src else ""))
    return subs


def build_cg_block(subs, indent, report):
    variants = {}
    lm_states, vl_states = set(), set()
    for keywords, src in subs:
        if keywords & DROP_KEYWORDS:
            continue
        lm = "LIGHTMAP_ON" in keywords
        vl = "VERTEXLIGHT_ON" in keywords
        if (lm, vl) in variants:
            continue
        variants[(lm, vl)] = convert_program(src)
        lm_states.add(lm)
        vl_states.add(vl)
    if not variants:
        raise ConvertError("no usable GLES variant")
    used = set()
    uniforms = {}
    warnings = []
    for p in variants.values():
        used |= p["used"]
        warnings += p["warnings"]
        for n, u in p["uniforms"].items():
            uniforms.setdefault(n, u)
    for w in sorted(set(warnings)):
        report.append("warning: " + w)
    lines = ["CGPROGRAM", "#pragma vertex vert", "#pragma fragment frag"]
    if len(lm_states) > 1:
        lines.append("#pragma multi_compile _ LIGHTMAP_ON")
    if len(vl_states) > 1:
        lines.append("#pragma multi_compile _ VERTEXLIGHT_ON")
    lines.append('#include "UnityCG.cginc"')
    # helpers in dependency order
    want = set(used)
    for h in list(want):
        want |= set(HELPER_DEPS.get(h, []))
    for h in ["sg_mod", "sg_ObjScale", "sg_World2Object", "sg_unity_Scale", "sg_FromGLClip"]:
        if h in want:
            lines += HELPERS[h].rstrip("\n").split("\n")
    for n, (prec, typ, arr) in sorted(uniforms.items()):
        lines.append("%s %s%s;" % (hl_type(prec, typ), n, "[%s]" % arr if arr else ""))

    def variant_lines(lm, vl):
        return emit_variant(variants[(lm, vl)], "")

    def by_vl(lm):
        if len(vl_states) > 1 and (lm, True) in variants and (lm, False) in variants:
            return ["#if defined(VERTEXLIGHT_ON)"] + variant_lines(lm, True) + ["#else"] + variant_lines(lm, False) + ["#endif"]
        return variant_lines(lm, False if (lm, False) in variants else True)

    if len(lm_states) > 1:
        lines += ["#if defined(LIGHTMAP_ON)"] + by_vl(True) + ["#else"] + by_vl(False) + ["#endif"]
    else:
        lines += by_vl(next(iter(lm_states)))
    lines.append("ENDCG")
    return "\n".join((indent + l) if l else l for l in lines)


HEADER = ("// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.\n"
          "// Exact transliteration of the original maths; edit the generator rather than this file where possible.\n")


def convert_shader(text, report):
    out, pos = [], 0
    count = 0
    while True:
        m = re.compile(r'Program\s+"vp"\s*\{').search(text, pos)
        if not m:
            out.append(text[pos:])
            break
        vp_end = find_block_end(text, m.end() - 1)
        fm = re.compile(r'\s*Program\s+"fp"\s*\{').match(text, vp_end)
        end = find_block_end(text, fm.end() - 1) if fm else vp_end
        line_start = text.rfind("\n", 0, m.start()) + 1
        indent = re.match(r"[ \t]*", text[line_start:m.start()]).group(0) or "  "
        subs = parse_subprograms(text[m.start():vp_end])
        out.append(text[pos:m.start()])
        out.append(build_cg_block(subs, indent, report).lstrip())
        pos = end
        count += 1
    res = "".join(out)
    if count:
        res = HEADER + res
    return res, count


def main(argv):
    if len(argv) >= 3 and argv[0] == "--all":
        src_root, dst_root = argv[1], argv[2]
        only = set(argv[3:])
        for dp, dn, fn in os.walk(src_root):
            for f in fn:
                if not f.endswith(".shader") or (only and f not in only):
                    continue
                p = os.path.join(dp, f)
                rel = os.path.relpath(p, src_root)
                rep = []
                try:
                    res, n = convert_shader(open(p, encoding="utf-8").read(), rep)
                except ConvertError as ex:
                    print("FAIL %s: %s" % (rel, ex))
                    continue
                q = os.path.join(dst_root, rel)
                os.makedirs(os.path.dirname(q), exist_ok=True)
                open(q, "w", encoding="utf-8", newline="\n").write(res)
                print("%s %s (%d programs)%s" % ("ok  " if n else "ff  ", rel, n, "".join("\n    " + r for r in rep)))
        return
    rep = []
    res, n = convert_shader(open(argv[0], encoding="utf-8").read(), rep)
    open(argv[1], "w", encoding="utf-8", newline="\n").write(res)
    for r in rep:
        print(r)


if __name__ == "__main__":
    main(sys.argv[1:])
