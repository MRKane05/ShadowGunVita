"""Compare the project's shaders (as committed before the port, git HEAD) with the shipped ones in the AssetRipper
decomp, and list what is missing. Writes Markdown to stdout.

Usage: python shader_report.py <project root> <decomp Assets dir> [git rev, default HEAD]
"""
import collections
import os
import re
import subprocess
import sys

name_re = re.compile(r'^\s*Shader\s+"([^"]+)"', re.M)
guid_re = re.compile(r"^guid:\s*([0-9a-f]{32})", re.M)
shref_re = re.compile(r"m_Shader:\s*\{fileID:\s*(-?\d+)(?:,\s*guid:\s*([0-9a-f]{32}),\s*type:\s*(\d+))?\}")
prop_re = re.compile(r'^\s*\[?[^\]\n]*\]?\s*(_\w+)\s*\("([^"]*)"\s*,\s*([^)]*\)?[^)=]*)\)\s*=\s*([^\n{]+?)(?:\s*\{[^}]*\})?\s*$', re.M)
STATE_KEYS = ["Blend", "ZWrite", "ZTest", "Cull", "ColorMask", "Offset", "AlphaTest"]


def git_show(root, rev, path):
    try:
        return subprocess.run(["git", "-C", root, "show", "%s:%s" % (rev, path.replace("\\", "/"))],
                              capture_output=True, check=True).stdout.decode("utf-8", "replace")
    except subprocess.CalledProcessError:
        return None


def props(txt):
    m = re.search(r"Properties\s*\{(.*?)\n\s*\}", txt, re.S)
    out = collections.OrderedDict()
    if not m:
        return out
    for line in m.group(1).split("\n"):
        pm = re.match(r'\s*(?:\[[^\]]*\]\s*)*(_\w+)\s*\("([^"]*)"\s*,\s*(.+?)\)\s*=\s*(.+?)\s*$', line)
        if pm:
            typ = pm.group(3).strip()
            typ = re.sub(r"\s+", "", typ).lower()
            if typ.startswith("range"):
                typ = typ + ")" if not typ.endswith(")") else typ
            default = re.sub(r"\s*\{\s*\}\s*$", "", pm.group(4)).strip()
            out[pm.group(1)] = (typ, re.sub(r"\s+", "", default).lower())
    return out


ALL_CMDS = STATE_KEYS + ["Lighting", "Fog", "SetTexture", "BindChannels", "Tags", "Name", "Pass", "Material",
                         "SeparateSpecular", "ColorMaterial", "LOD", "SubShader", "Category", "Fallback", "UsePass"]


def _num(m):
    return repr(float(m.group(0))).rstrip("0").rstrip(".") if m.group(0) else m.group(0)


def states(txt):
    """set of normalised render-state commands (ShaderLab) + queue / light mode tags"""
    body = re.sub(r"CGPROGRAM.*?ENDCG", "", txt, flags=re.S)
    body = re.sub(r'Program\s+"\w+"\s*\{.*', "", body, flags=re.S)  # decomp: program blobs
    body = re.sub(r"//[^\n]*", "", body)
    m = re.search(r"Properties\s*\{.*?\n\s*\}", body, re.S)
    if m:
        body = body[:m.start()] + body[m.end():]
    tags = " ".join(re.findall(r"Tags\s*\{([^}]*)\}", body))
    body_nostr = re.sub(r'"[^"]*"', '""', body)
    res = set()
    stop = r"(?=\s+(?:%s)\b|\s*[\n{}]|\s*$)" % "|".join(ALL_CMDS)
    for k in STATE_KEYS:
        for m in re.finditer(r"\b%s\s+(.*?)%s" % (k, stop), body_nostr, re.I):
            v = re.sub(r"\s+", " ", m.group(1)).strip().lower()
            v = re.sub(r"-?\d*\.\d+|-?\d+", _num, v)
            res.add("%s %s" % (k, v))
    for m in re.finditer(r'"Queue"\s*=\s*"([^"]+)"', tags, re.I):
        res.add("Queue " + m.group(1).lower())
    for m in re.finditer(r'"LightMode"\s*=\s*"([^"]+)"', tags, re.I):
        res.add("LightMode " + m.group(1).lower())
    return res


def classify(txt):
    if "DummyShaderTextExporter" in txt:
        return "stub"
    if "Reconstructed" in txt:
        return "placeholder"
    if "CGPROGRAM" in txt:
        return "source"
    return "fixed-function"


def walk(root, ext):
    for dp, dn, fn in os.walk(root):
        for f in fn:
            if f.endswith(ext):
                yield os.path.join(dp, f)


def main(proj, dec, rev="HEAD"):
    assets = os.path.join(proj, "Assets")
    # project shaders at <rev>
    files = subprocess.run(["git", "-C", proj, "ls-tree", "-r", "--name-only", rev, "--", "Assets"],
                           capture_output=True, check=True).stdout.decode().split("\n")
    old = {}
    for f in files:
        if f.endswith(".shader"):
            t = git_show(proj, rev, f)
            m = name_re.search(t)
            old[m.group(1)] = (f, t)
    dshaders = {}
    dguid = {}
    for p in walk(dec, ".shader"):
        t = open(p, encoding="utf-8", errors="replace").read()
        m = name_re.search(t)
        dshaders[m.group(1)] = (os.path.relpath(p, dec), t)
        meta = p + ".meta"
        if os.path.exists(meta):
            g = guid_re.search(open(meta, encoding="utf-8").read())
            if g:
                dguid[g.group(1)] = m.group(1)
    # material users in the decomp
    users = collections.defaultdict(list)
    for p in walk(dec, ".mat"):
        t = open(p, encoding="utf-8", errors="replace").read()
        m = shref_re.search(t)
        if m and m.group(2) in dguid:
            users[dguid[m.group(2)]].append(os.path.basename(p)[:-4])
    # project material -> shader at <rev>
    pguid = {}
    for f in files:
        if f.endswith(".shader.meta"):
            t = git_show(proj, rev, f) or ""
            g = guid_re.search(t)
            if g:
                pguid[g.group(1)] = name_re.search(git_show(proj, rev, f[:-5])).group(1)
    pmat = {}
    for f in files:
        if f.endswith(".mat"):
            t = git_show(proj, rev, f) or ""
            m = shref_re.search(t)
            if m:
                pmat[os.path.basename(f)[:-4]] = pguid.get(m.group(2), "built-in fileID %s" % m.group(1)) if m.group(2) in pguid else "Unity built-in (fileID %s)" % m.group(1)

    out = []
    w = out.append
    shared = sorted(set(old) & set(dshaders), key=lambda n: (classify(old[n][1]), n))
    w("## Game shaders that are in the project (%d)\n" % len(shared))
    w("Status of each project shader **before** this port, compared with the shipped 1.3.5 shader in the rip.\n")
    w("| Shader | File | Old project version | Materials (rip) | Differences found |")
    w("|---|---|---|---|---|")
    labels = {"placeholder": "Placeholder (generic stand-in)", "stub": "AssetRipper dummy (Lambert stub)",
              "source": "Hand port / sample source", "fixed-function": "Fixed function"}
    for n in shared:
        of, ot = old[n]
        df, dt = dshaders[n]
        diffs = []
        po, pd = props(ot), props(dt)
        missing = [k for k in pd if k not in po]
        extra = [k for k in po if k not in pd]
        changed = [k for k in pd if k in po and po[k] != pd[k]]
        if missing:
            diffs.append("missing properties " + ", ".join("`%s`" % k for k in missing))
        if extra:
            diffs.append("extra properties " + ", ".join("`%s`" % k for k in extra))
        if changed:
            diffs.append("property type/default differs: " + ", ".join("`%s`" % k for k in changed))
        so, sd = states(ot), states(dt)
        if so != sd:
            a = sorted(sd - so)
            b = sorted(so - sd)
            if a:
                diffs.append("game has: " + "; ".join(a))
            if b:
                diffs.append("project had: " + "; ".join(b))
        c = classify(ot)
        if c in ("source",) and "DecodeLightmap" in ot:
            diffs.append("lightmap through DecodeLightmap() (game: plain sample x2; same only with dLDR lightmap encoding)")
        if c == "placeholder":
            diffs.insert(0, "generic shader, none of the original effect")
        if c == "stub":
            diffs.insert(0, "renders a lit diffuse surface, none of the original effect")
        if not diffs:
            diffs.append("none (same behaviour)" if c == "fixed-function" else "same state and properties; maths not compared line by line")
        w("| %s | `%s` | %s | %d | %s |" % (n, of[len("Assets/"):], labels[c], len(users.get(n, [])), "<br>".join(diffs)))
    w("")
    only_p = sorted(set(old) - set(dshaders))
    w("## Project-only shaders (%d)\n" % len(only_p))
    for n in only_p:
        w("- **%s** (`%s`)" % (n, old[n][0][len("Assets/"):]))
    w("")
    only_d = sorted(set(dshaders) - set(old))
    used = [n for n in only_d if users.get(n)]
    unused = [n for n in only_d if not users.get(n)]
    w("## Shaders in the rip that the project does not have (%d)\n" % len(only_d))
    w("All are Unity 3.5 built-in shaders that shipped inside the game's data.\n")
    w("Used by the game's materials (%d):\n" % len(used))
    w("| Shader | Rip file | Game materials | What the project uses instead |")
    w("|---|---|---|---|")
    for n in used:
        mats = sorted(set(users[n]))
        instead = sorted(set(pmat.get(m, "material missing") for m in mats))
        w("| %s | `%s` | %s | %s |" % (n, dshaders[n][0], ", ".join(mats), ", ".join(instead)))
    w("")
    w("Not used by any material (%d): %s\n" % (len(unused), ", ".join("%s (`%s`)" % (n, os.path.basename(dshaders[n][0])) for n in unused)))
    print("\n".join(out))


if __name__ == "__main__":
    main(*sys.argv[1:])
