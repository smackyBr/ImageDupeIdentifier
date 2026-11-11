# Security

Image Dupe Identifier handles people's personal photo libraries, so it is built to be
boring and predictable from a security point of view.

## Promises

1. **Fully offline.** The app never connects to the internet or any network. No telemetry,
   no update checks, no cloud features, no accounts.
2. **No admin rights.** It runs as a normal user and only touches folders you pick.
3. **No permanent deletes.** Removing a photo sends it to the Recycle Bin (Windows) or
   Trash (Mac), so every delete can be undone.
4. **No hidden code.** Every release is built in public by GitHub Actions from the source
   in this repo, with checksums and a signed build-provenance attestation.

## How the promises are enforced (not just written down)

| Promise | Enforcement |
|---|---|
| Offline | `BannedSymbols.txt` + Microsoft's BannedApiAnalyzers: any use of `HttpClient`, sockets, DNS, `WebClient`, etc. in app code is a **compile error**. |
| No launching other programs | `System.Diagnostics.Process` is banned the same way; any future exception needs a reviewed, commented suppression. |
| No permanent deletes | `File.Delete`, `FileInfo.Delete`, `Directory.Delete` are banned in app code. |
| No runtime code loading | `Assembly.LoadFrom/LoadFile/Load(byte[])` are banned. |
| No tampered dependencies | `packages.lock.json` pins every package's exact version and content hash; CI fails if anything differs. |
| No known-vulnerable dependencies | NuGet Audit fails the build on any known vulnerability; Dependabot opens fix PRs. |
| Code scanning | GitHub CodeQL (`security-extended`) runs on every change and weekly. |
| Trustworthy downloads | `SHA256SUMS.txt` and GitHub build-provenance attestation on every release. |

## Verifying a download

```bash
# Checksum (compare with SHA256SUMS.txt on the release page)
shasum -a 256 ImageDupe-0.1.0-osx-arm64.zip          # macOS
certutil -hashfile ImageDupe-0.1.0-win-x64.zip SHA256 # Windows

# Proof it was built by this repository (needs GitHub CLI)
gh attestation verify ImageDupe-0.1.0-win-x64.zip --repo <owner>/ImageDupeIdentifier
```

## Reporting a vulnerability

Please use **Security > Report a vulnerability** on this repository (private to the
maintainer). Don't open a public issue for security problems.
