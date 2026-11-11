#!/usr/bin/env bash
# Wraps `dotnet publish` output into a double-clickable macOS .app and zips it.
# Usage: build/package-macos.sh <rid> <version> <publish-dir> <out-dir>
set -euo pipefail

RID="$1"
VERSION="$2"
PUBLISH_DIR="$3"
OUT_DIR="$4"

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
APP_NAME="Image Dupe Identifier"
APP="$OUT_DIR/$APP_NAME.app"

rm -rf "$APP"
mkdir -p "$APP/Contents/MacOS" "$APP/Contents/Resources"

cp -R "$PUBLISH_DIR"/. "$APP/Contents/MacOS/"
sed "s/__VERSION__/$VERSION/g" "$SCRIPT_DIR/macos/Info.plist" > "$APP/Contents/Info.plist"
chmod +x "$APP/Contents/MacOS/ImageDupe"

# Ad-hoc signature (required to run on Apple Silicon). This is NOT Apple notarization;
# users will see a one-time "unidentified developer" prompt. See docs/SETUP.md.
codesign --force --deep --sign - "$APP"

# ditto preserves the bundle structure and permissions correctly.
(cd "$OUT_DIR" && ditto -c -k --keepParent "$APP_NAME.app" "ImageDupe-$VERSION-$RID.zip")
rm -rf "$APP"

echo "Created $OUT_DIR/ImageDupe-$VERSION-$RID.zip"
