import argparse
import subprocess
import sys

nuitka_args = [
    "--standalone",  # Create a standalone folder with all the required files and a .exe
    "--remove-output",  # Remove the build folder after creating the dist folder
    "--assume-yes-for-downloads",  # Assume yes for all downloads
    "--include-data-dir=data=data",  # Copy over the data folder to the output directory
    "--output-dir=Release",  # Output directory
    #'--noinclude-numba-mode=nofollow  ',
    # Compile the following directories as modules (for dynamic imports)
    # '--include-module=data.poses.python',
    # '--include-module=data.gestures.python',
    # '--include-module=lib.view.elements',
    # '--include-module=data.speech_submodes.python',
    #'--include-package=TTS',
    #'--include-package=torch',
    # Include the following plugins
    # '--user-plugin=plugins/mediapipe.py',
    #'--user-plugin=plugins/sounddevice.py'
    #'--user-plugin=plugins/TorchPlugin.py',
    #'--user-plugin=plugins/NumpyPlugin.py',
]


def build() -> None:
    """
    Sets up an argument parser for building a MotionInput project with Nuitka, providing options for
    link time optimization (LTO), console visibility, and target file specification.

    Command line arguments:
        * --lto: Use link time optimization. Defaults to False.
        * --console: Show console when running the built executable. Defaults to False.
        * target_file: Target file to build. Defaults to "motioninput.py".

    :Example:

        Run the following command in the terminal:

        .. code-block:: bash

            python build.py --lto --console motioninput.py

    :return: None
    """
    parser = argparse.ArgumentParser(description="Build motion input with Nuitka.")
    parser.add_argument(
        "--lto", action="store_true", help="Use link time optimisation. Default: False"
    )
    # parser.add_argument('--console', action='store_true',
    #                     help='Show console when running the built executable. Default: False')
    parser.add_argument("target_file", type=str, help="Target file to build.")
    args = parser.parse_args()

    print(args)

    if args.lto:
        nuitka_args.append("--lto=yes")
    else:
        nuitka_args.append("--lto=no")

    # if not args.console:
    #     nuitka_args.append('--windows-disable-console')

    subprocess.call([sys.executable, "-m", "nuitka"] + nuitka_args + [args.target_file])


if __name__ == "__main__":
    build()
