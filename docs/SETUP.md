# Developer setup & release guide

First-time setup takes about 30–45 minutes. After that, the daily loop is a few commands.

---

## Part 1 — Install the tools (once per computer)

| Tool | Where | Check it worked |
|---|---|---|
| Git | <https://git-scm.com/downloads> (Mac: run `xcode-select --install`) | `git --version` |
| .NET 10 SDK | <https://dotnet.microsoft.com/download/dotnet/10.0> | `dotnet --list-sdks` shows `10.0.x` |
| VS Code | <https://code.visualstudio.com> | — |
| GitHub CLI | <https://cli.github.com> (Windows: `winget install GitHub.cli`, Mac: `brew install gh`) | `gh --version` |

Open this folder in VS Code. It will prompt **"install recommended extensions"** → click
**Install**. That adds C# Dev Kit (solution explorer, debugger, test explorer), the Avalonia
XAML extension, and GitHub integration.

### Tell Git who you are (once)

Use GitHub's private **noreply** email so your real email never appears in public commit
history. Find it at GitHub → Settings → Emails (looks like `12345678+username@users.noreply.github.com`).

```bash
git config --global user.name  "Happy hap"
git config --global user.email "12345678+yourusername@users.noreply.github.com"
git config --global init.defaultBranch main
```

### Secure your GitHub account (once — this is the #1 anti-hacking step)

Your GitHub account is the keys to every release people download. If someone takes it over,
they can publish a malicious build under your name.

1. GitHub → Settings → **Password and authentication** → enable **two-factor
   authentication** (an authenticator app or a passkey; not SMS).
2. Save the recovery codes somewhere offline.

---

## Part 2 — First build on your machine

From the repo folder (VS Code → Terminal → New Terminal):

```bash
dotnet restore     # downloads packages AND creates packages.lock.json files
dotnet build
dotnet test        # should show all tests passing
dotnet run --project src/ImageDupe.App   # the app window opens
```

`dotnet restore` is the only step that needs the internet (to download packages from
nuget.org). The built app itself never does.

> The `packages.lock.json` files it creates are **important** — they record the exact
> version and hash of every package. Commit them. CI refuses to build if they're missing
> or don't match.

In VS Code you can also: **F5** to debug the app, **Ctrl/Cmd+Shift+B** to build, and the
beaker icon (Testing panel) to run tests individually.

---

## Part 3 — Put it on GitHub (once)

```bash
gh auth login          # choose GitHub.com → HTTPS → log in with browser

git init
git add .
git commit -m "Initial project framework"

gh repo create ImageDupeIdentifier --public --source . --push \
  --description "Offline duplicate photo finder for Windows and Mac"
```

That creates the repo, uploads your code, and connects your folder to it. Open it with
`gh repo view --web`. Within a minute the **Actions** tab shows CI building on Windows and Mac.

**Public or private?** Public is recommended: anyone can download releases without a GitHub
account, CodeQL scanning is free, and your "it's offline" claim can be verified by
reading the code. Private repos only let invited collaborators download releases.

### Turn on repo protections (once, in the browser)

Repo → **Settings**:

1. **Code security** (Advanced Security section) → enable:
   - Dependabot alerts, Dependabot security updates
   - Secret scanning + **Push protection** (blocks you from accidentally committing a password/key)
   - Private vulnerability reporting
2. **Actions → General**:
   - Workflow permissions → **Read repository contents** (the default; workflows ask for more only where needed)
   - Fork pull request workflows → **Require approval for all external contributors**
3. **Rules → Rulesets → New branch ruleset**, name `protect main`, target the default branch:
   - ✅ Restrict deletions, ✅ Block force pushes
   - ✅ Require status checks to pass → add `Build & test (windows-latest)` and `Build & test (macos-latest)`
     (they appear in the list after CI has run once)
   - Optional for a solo project: ✅ Require a pull request before merging
4. **Rules → Rulesets → New tag ruleset**, name `protect releases`, target pattern `v*`:
   - ✅ Restrict creations, updates, deletions — then add yourself (Repository admin role) to the bypass list.
     Now only you can create release tags.

---

## Part 4 — Day-to-day workflow

```bash
git switch -c feature/scan-screen      # 1. make a branch for the change
#   ...write code, dotnet build, dotnet test...
git add .
git commit -m "Add scan screen"        # 2. save a snapshot (do this often)
git push -u origin feature/scan-screen # 3. upload the branch
gh pr create --fill                    # 4. open a pull request; CI tests it on Win + Mac
gh pr merge --squash --delete-branch   # 5. once green, merge into main
git switch main && git pull            # 6. update your local main
```

VS Code's **Source Control** panel (branch icon) does the same with buttons if you prefer.

Dependabot will open its own pull requests for package updates. If CI is green, merge them.

---

## Part 5 — Publishing a release for other people

1. Make sure `main` is green in the Actions tab.
2. Tag the version and push the tag:

   ```bash
   git switch main && git pull
   git tag v0.1.0
   git push origin v0.1.0
   ```

3. The **Release** workflow (~10 min) tests, builds Windows x64/ARM64 and Mac
   Apple-Silicon/Intel, writes `SHA256SUMS.txt`, signs a build-provenance attestation, and
   creates the release page with the zips attached.
4. Share the link: `https://github.com/<you>/ImageDupeIdentifier/releases/latest`

Versions follow `MAJOR.MINOR.PATCH`: bump PATCH for fixes (v0.1.1), MINOR for features
(v0.2.0), and go to v1.0.0 when you consider it stable.

If a release build fails, delete the tag, fix, and re-tag:
`git push --delete origin v0.1.0 && git tag -d v0.1.0`.

---

## Part 6 — Later: removing the "unknown developer" warnings

Unsigned apps work fine but show a one-time warning. When you're ready to share widely:

- **Mac:** Apple Developer Program ($99/yr) → Developer ID certificate + notarization in the
  release workflow. Removes the Gatekeeper prompt.
- **Windows:** a code-signing service (e.g. Azure Trusted Signing) in the release workflow.
  Reduces SmartScreen warnings.

Both plug into `release.yml` using GitHub encrypted **secrets** — never commit certificates.

---

## VS Code vs Visual Studio 2026

VS Code is fully capable for this project on both Windows and Mac, and everything above
uses the `dotnet` CLI so nothing depends on the editor. Visual Studio 2026 (Windows only)
has a richer XAML designer; you can open the same `ImageDupeIdentifier.slnx` in it any time.
