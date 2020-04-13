N64 GFX Cookie 0.2
by LuigiBlood

This tool deals with CI4/CI8 N64 graphics data. It can render graphics, export them to PNG (with alpha channel as RGBA5551 does support), and import PNGs back into CI4/CI8 format and fits them into the same palette.
The expected file format:

CI4 Format:
Offset - Size  - Description
0x0000 - 0x20  - RGBA5551 16 color palette Data
0x0200 - ?     - CI4 Graphics Data (optional)

CI8 Format:
Offset - Size  - Description
0x0000 - 0x200 - RGBA5551 256 color palette Data
0x0200 - ?     - CI8 Graphics Data (optional)