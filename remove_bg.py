"""
remove_bg.py
Removes backgrounds from all PNG sprites in Assets/Sprites/UI/ and Assets/Sprites/Environment/
using the rembg library. Overwrites files in-place.

Install deps first:
    pip install rembg pillow

Run from the project root:
    python remove_bg.py
"""

from pathlib import Path
from rembg import remove
from PIL import Image
import io

# ── Folders to process ────────────────────────────────────────────────────────
TARGET_DIRS = [
    Path("Assets/Sprites/UI"),
    Path("Assets/Sprites/Environment"),
]

# Only these filenames need bg removal (sky_bg is intentional, skip it)
SKIP_FILES = {
    "sky_bg.png",           # intentional background image
    "Snow-tile-low-res.png",# tilemap tile — keep as-is
}

def process(png_path: Path) -> None:
    if png_path.name in SKIP_FILES:
        print(f"  SKIP  {png_path}")
        return

    print(f"  Processing  {png_path} ...", end=" ", flush=True)
    input_data = png_path.read_bytes()
    output_data = remove(input_data)          # rembg does the heavy lifting

    # Ensure RGBA so Unity reads transparency correctly
    img = Image.open(io.BytesIO(output_data)).convert("RGBA")
    img.save(png_path, format="PNG")
    print("done")

def main():
    processed = 0
    for folder in TARGET_DIRS:
        if not folder.exists():
            print(f"[WARN] folder not found: {folder}")
            continue
        pngs = list(folder.glob("*.png"))
        if not pngs:
            print(f"[INFO] no PNGs in {folder}")
            continue
        print(f"\n--- {folder} ({len(pngs)} files) ---")
        for png in sorted(pngs):
            process(png)
            processed += 1

    print(f"\n✅ Done — {processed} file(s) processed.")

if __name__ == "__main__":
    main()
