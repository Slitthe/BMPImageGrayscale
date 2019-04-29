using System;
using System.IO;

namespace BMPImageGrayscale
{
    class Program
    {
        static void Main(string[] args)
        {
            // input/output file paths
            string inputFilePath = args[1];
            string outputFilePath = args[2];

            // file represented as array of bytes
            byte[] readImg = File.ReadAllBytes(inputFilePath);

            // bits per pixel -- to know how many elements to skip
            int bitsPerPixels = (readImg[0x1C + 0x1] << 8) | readImg[0x1C];
            int bytesPerPixel = bitsPerPixels / 8;

            // get the bitmap array start location -- to know where to start reading
            int imgArrOffset = (readImg[0xA + 0x3] << 24) | (readImg[0xA + 0x2] << 16) | (readImg[0xA + 0x1] << 8) | readImg[0xA];

            // width & height of the image in pixels
            int height = (readImg[0x16 + 0x3] << 24) | (readImg[0x16 + 0x2] << 16) | (readImg[0x16 + 0x1] << 8) | readImg[0x16];
            int width = (readImg[0x12 + 0x3] << 24) | (readImg[0x12 + 0x2] << 16) | (readImg[0x12 + 0x1] << 8) | readImg[0x12];

            // grayness level 1 --> all gray 0- -> no color change
            float graynessLevel = 1f; // default value if no argument is provided
            float.TryParse(args[0], out graynessLevel);

            // calculate the total row width (including the pixel padding)
            int rowWidth = width * bytesPerPixel;
            while(rowWidth % 4 != 0)
            {
                rowWidth++;
            }
            // ^^^^^^^^^^^^^^^GOOD UNTIL THIS POINT ^^^^^^^^^^^^^^^^^^^^


            int currentArrImgOffset;
            for(int k = 0; k < height; k++)
            {
                currentArrImgOffset = imgArrOffset + (k * rowWidth);

                for (int  l = 0; l < width; l++)
                {
                    int currentIndexStart = (currentArrImgOffset + l * bytesPerPixel);
                    float avg = 0;
                    avg = (avg / bytesPerPixel);

                    for (int j = 0; j < bytesPerPixel; j++)
                    {
                        var prevVal = readImg[currentIndexStart + j];
                        var diff = avg - prevVal;
                        readImg[currentIndexStart + j] = (byte)(prevVal + diff * graynessLevel);
                    }
                }
            }

     
            File.WriteAllBytesAsync(outputFilePath, readImg);
        }

    }
}
