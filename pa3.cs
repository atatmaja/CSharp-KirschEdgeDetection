using System;
using static System.Console;
using System.IO;
using System.Linq;

namespace Bme121.Pa3
{
    /// <StudentPlan>Biomedical Engineering</StudentPlan>
    /// <StudentDept>Department of Systems Design Engineering</StudentDept>
    /// <StudentInst>University of Waterloo</StudentInst>
    /// <StudentName>Atmaja, Austin</StudentName>
    /// <StudentUserID>atatmaja</StudentUserID>
    /// <StudentAcknowledgements>
    /// I declare that, except as acknowledged below, this is my original work.
    /// Acknowledged contributions of others:
    /// </StudentAcknowledgements>
    
    static partial class Program
    {
        static void Main( )
        {
            string inputFile  = @"21_training.csv";
            string outputFile = @"21_training_edges.csv";

            // Read the input image from its csv file.
            // TO DO:
            FileStream inCSVFile = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(inCSVFile);

            int rows = int.Parse(sr.ReadLine()); // read first line, convert to int rows
            int cols = int.Parse(sr.ReadLine()); // read second line, convert to int cols
            Color[,] inImage = new Color[rows, cols];
            Color[,] outImage = new Color[rows, cols];

            int[,] CSVInput = new int[rows, cols*4];
            for (int i = 0; i < rows; i++)
            {
                string Line = sr.ReadLine();
                string[] pixelValInput = Line.Split(',');
                for (int j = 0; j < cols*4; j++)
                {
                    CSVInput[i, j] = int.Parse(pixelValInput[j]);
                }
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols*4; j = j + 4)
                {
                    inImage[i, j / 4] = Color.FromArgb(CSVInput[i, j], CSVInput[i, j+1], CSVInput[i, j+2], CSVInput[i, j+3]);
                }
            }

            // Generate the output image using Kirsch edge detection.
            // TO DO:

            outImage = inImage;

            for (int i = 1; i < rows - 1; i++)
            {
                for (int j = 1; j < cols - 1; j++)
                {
                    outImage[i, j] = GetKirschEdgeValue(inImage[i - 1, j - 1], inImage[i - 1, j], inImage[i - 1, j + 1],
                        inImage[i, j - 1], inImage[i, j], inImage[i - 1, j + 1],
                        inImage[i + 1, j - 1], inImage[i + 1, j], inImage[i + 1, j + 1]);
                }
            }

            // Write the output image to its csv file.
            // TO DO:
        
            FileStream outCSVFile = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
            StreamWriter swriter = new StreamWriter(outCSVFile);

            int[,] finalOutput = new int[rows, cols*4];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    finalOutput[i, 4*j] = outImage[i, j].A;
                    finalOutput[i, 4*j + 1] = outImage[i, j].R;
                    finalOutput[i, 4*j + 2] = outImage[i, j].G;
                    finalOutput[i, 4*j + 3] = outImage[i, j].B;
                }
            }

            swriter.WriteLine(rows);
            swriter.WriteLine(cols);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols*4; j++)
                {
                    if (j < cols*4 - 1)
                    {
                        swriter.Write(finalOutput[i, j] + ",");
                    }
                    else
                    {
                        swriter.WriteLine(finalOutput[i, j]);
                    }
                }
            }

            swriter.Dispose();
            outCSVFile.Dispose();
            



        }

        // This method computes the Kirsch edge-detection value for pixel color
        // at the centre location given the centre-location pixel color and the
        // colors of its eight neighbours.  These are numbered as follows.
        // The resulting color has the same alpha as the centre pixel, 
        // and Kirsch edge-detection intensities which are computed separately
        // for each of the red, green, and blue components using its eight neighbours.
        // c1 c2 c3
        // c4    c5
        // c6 c7 c8
        static Color GetKirschEdgeValue( 
            Color c1, Color c2,     Color c3, 
            Color c4, Color centre, Color c5, 
            Color c6, Color c7,     Color c8 )
        {
           
            int newRedIntensity = GetKirschEdgeValue(c1.R, c2.R, c3.R, c4.R, c5.R, c6.R, c7.R, c8.R);
            int newBlueIntensity = GetKirschEdgeValue(c1.B, c2.B, c3.B, c4.B, c5.B, c6.B, c7.B, c8.B);
            int newGreenIntensity = GetKirschEdgeValue(c1.G, c2.G, c3.G, c4.G, c5.G, c6.G, c7.G, c8.G);
            int centreAlpha = centre.A;
            // TO DO: (Replace the following line.)
            Color newCentreColor = Color.FromArgb(centreAlpha, newRedIntensity, newGreenIntensity, newBlueIntensity);
            return newCentreColor;
        }
        
        // This method computes the Kirsch edge-detection value for pixel intensity
        // at the centre location given the pixel intensities of the eight neighbours.
        // These are numbered as follows.
        // i1 i2 i3
        // i4    i5
        // i6 i7 i8
        static int GetKirschEdgeValue( 
            int i1, int i2, int i3, 
            int i4,         int i5, 
            int i6, int i7, int i8 )
        {
            int[] intensitySum = new int[8];
            intensitySum[0] = 5*(i1 + i2 + i3) + -3*(i4 + i5 + i6 + i7 + i8);
            intensitySum[1] = 5 * (i5 + i2 + i3) + -3 * (i4 + i1 + i6 + i7 + i8);
            intensitySum[2] = 5 * (i8 + i5 + i3) + -3 * (i4 + i1 + i6 + i7 + i2);
            intensitySum[3] = 5 * (i5 + i7 + i8) + -3 * (i4 + i1 + i6 + i2 + i3);
            intensitySum[4] = 5 * (i6 + i7 + i8) + -3 * (i4 + i5 + i1 + i2 + i3);
            intensitySum[5] = 5 * (i4 + i6 + i7) + -3 * (i1 + i5 + i2 + i3 + i8);
            intensitySum[6] = 5 * (i1 + i4 + i6) + -3 * (i2 + i5 + i3 + i7 + i8);
            intensitySum[7] = 5 * (i1 + i2 + i4) + -3 * (i3 + i5 + i6 + i7 + i8);

            int intensity = intensitySum.Max();

            if (intensity < 0)
            {
                return 0;
            }
            else if (intensity > 255)
            {
                return 255;
            }
            else
            {
                return intensity;
            }
        }
    }
    
    // Implementation of part of System.Drawing.Color.
    // This is needed because .Net Core doesn't seem to include the assembly 
    // containing System.Drawing.Color even though docs.microsoft.com claims 
    // it is part of the .Net Core API.
    struct Color
    {
        int alpha;
        int red;
        int green;
        int blue;
        
        public int A { get { return alpha; } }
        public int R { get { return red;   } }
        public int G { get { return green; } }
        public int B { get { return blue;  } }
        
        public static Color FromArgb( int alpha, int red, int green, int blue )
        {
            Color result = new Color( );
            result.alpha = alpha;
            result.red   = red;
            result.green = green;
            result.blue  = blue;
            return result;
        }
    }
}
