using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace SecuringSynoptic
{
    public class EncryptImage
    {
        public static string Encrypt(string pass, string data, string path, Bitmap img)
        {

            // Declare the password that will allow you to retrieve the encrypted data later
            //string _PASSWORD = "password";
            string _PASSWORD = pass;

            // The String data to conceal on the image
            //string _DATA_TO_HIDE = "Hello, no one should know that my password is 12345";
            string _DATA_TO_HIDE = data;

            // Declare the path where the original image is located
            string pathOriginalImage = path;
            //string pathOriginalImage = path;

            // Declare the new name of the file that will be generated with the hidden information
            //string pathResultImage = @"C:\Users\matth\Desktop\nevera_with_hidden.png";
            string testPath = path.Remove(path.Length - 4) + "_with_hidden.jpg";
            string pathResultImage = testPath;


            // Create an instance of the SteganographyHelper
            SteganographyHelper helper = new SteganographyHelper();

            // Encrypt your data to increase security
            // Remember: only the encrypted data should be stored on the image
            //string encryptedData = CipherHelper.Encrypt(_DATA_TO_HIDE, _PASSWORD);
            string encryptedData = CipherHelper.Encrypt(_DATA_TO_HIDE, _PASSWORD);

            // Create an instance of the original image without indexed pixels
            Bitmap originalImage = SteganographyHelper.CreateNonIndexedImage(Image.FromFile(pathOriginalImage));
            // Conceal the encrypted data on the image !
            Bitmap imageWithHiddenData = SteganographyHelper.MergeText(encryptedData, originalImage);

            // Save the image with the hidden information somewhere :)
            // In this case the generated file will be image_example_with_hidden_information.png
            imageWithHiddenData.Save(pathResultImage);

            string work = "Index";
            return work;
        }
    }
}
