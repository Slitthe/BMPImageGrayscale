using System.IO;

namespace BMPImageGrayscale
{
    class Program
    {
        static void Main(string[] args)
        {
            int k, l, j, currentArrImgOffset, currentIndexStart;
            float avg, diff;
            byte prevVal;

            string inputFilePath = args[1];
            string outputFilePath = args[2];

            byte[] inputImageData = File.ReadAllBytes(inputFilePath);

            int bitsPerPixels = (inputImageData[0x1C + 0x1] << 8) | inputImageData[0x1C];
            int bytesPerPixel = bitsPerPixels / 8;

            // where the pixel array starts at
            int imgArrOffset = (inputImageData[0xA + 0x3] << 24) | (inputImageData[0xA + 0x2] << 16) | (inputImageData[0xA + 0x1] << 8) | inputImageData[0xA];


            int imageHeight = (inputImageData[0x16 + 0x3] << 24) | (inputImageData[0x16 + 0x2] << 16) | (inputImageData[0x16 + 0x1] << 8) | inputImageData[0x16];
            int imageWidth = (inputImageData[0x12 + 0x3] << 24) | (inputImageData[0x12 + 0x2] << 16) | (inputImageData[0x12 + 0x1] << 8) | inputImageData[0x12];

            float graynessLevel = 1f;
            float.TryParse(args[0], out graynessLevel);

            // adds the padding (if required) for the row (byes of row divisible by 4)
            int rowWidth = imageWidth * bytesPerPixel;
            while(rowWidth % 4 != 0)
            {
                rowWidth++;
            }



            for(k = 0; k < imageHeight; k++)
            {
                currentArrImgOffset = imgArrOffset + (k * rowWidth); // location where the current row starts

                for (l = 0; l < imageWidth; l++)
                {
                    currentIndexStart = (currentArrImgOffset + l * bytesPerPixel); // current pixel location (absolute to the file)
                    avg = 0;
                    for (j = 0; j < bytesPerPixel; j++)
                    {
                        avg += inputImageData[currentIndexStart + j];
                    }
                    avg = (avg / bytesPerPixel);

                    for (j = 0; j < bytesPerPixel; j++)
                    {
                        prevVal = inputImageData[currentIndexStart + j];
                        diff = avg - prevVal; // used for different grayness levels
                        inputImageData[currentIndexStart + j] = (byte)(prevVal + diff * graynessLevel); // sets the individual pixels
                    }
                }
            }

            File.WriteAllBytesAsync(outputFilePath, inputImageData);

            return;
        }

    }
}
