﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;

namespace file_encypt_decrypt
{
    public partial class Form1 : Form
    {

        private String KeyText = "";
        private String FilePath = "";

        public Form1()
        {
            InitializeComponent();
        }

        // Set the value of the File Name textbox to a string for use later
        private void FileTextBox_TextChanged(object sender, EventArgs e)
        {
            if (FileTextBox.Text != null)
            {
                this.FilePath = FileTextBox.Text;
            }
        }

        // Set the value of the Key textBox to a string for use later
        private void KeyTextBox_TextChanged(object sender, EventArgs e)
        {
            if (KeyTextBox.Text != null)
            {
                this.KeyText = KeyTextBox.Text;
            }
        }

        private void OpenFileButton_Click(object sender, EventArgs e)
        {
            // Use openFileDialog tool (in designer) to create openFileDialog1
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
                FileTextBox.Text = openFileDialog1.FileName;
            this.Invalidate();
        }

        private void EncryptButton_Click(object sender, EventArgs e)
        {
            EncryptFile();
            this.Invalidate();
        }

        private void DecryptButton_Click(object sender, EventArgs e)
        {
            DecryptFile();
            this.Invalidate();
        }
        
        private void EncryptFile()
        {
            try
            {
                // Generate variables needed to encrypt from user entry
                string inName = this.FilePath;
                string outName = this.FilePath + ".des";
                byte[] desKey = this.keytoByteArray();
                byte[] desIV = this.keytoByteArray();

                //Create the file streams to handle the input and output files.
                FileStream fin = new FileStream(inName, FileMode.Open, FileAccess.Read);
                FileStream fout = new FileStream(outName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);

                //Create variables to help with read and write. 
                byte[] bin = new byte[100];     //This is intermediate storage for the encryption. 
                long rdlen = 0;                 //This is the total number of bytes written. 
                long totlen = fin.Length;       //This is the total length of the input file. 
                int len;                        //This is the number of bytes to be written at a time.

                DES des = new DESCryptoServiceProvider();
                CryptoStream encStream = new CryptoStream(fout, des.CreateEncryptor(desKey, desIV), CryptoStreamMode.Write);

                Console.WriteLine("Encrypting...");

                //Read from the input file, then encrypt and write to the output file. 
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 100);
                    encStream.Write(bin, 0, len);
                    rdlen = rdlen + len;
                    Console.WriteLine("{0} bytes processed", rdlen);
                }

                Console.WriteLine("Encryption complete.");
                encStream.Close();
                fout.Close();
                fin.Close(); 
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
            }
        }

        private void DecryptFile()
        {
            try
            {
                // Generate variables needed to encrypt from user entry
                string inName = this.FilePath;
                if (Path.GetExtension(inName) != ".des")
                {
                    throw new Exception("Not a .des file.");
                }
                string outName = Path.GetFileNameWithoutExtension(FilePath);  // Remove ".des" extension
                byte[] desKey = this.keytoByteArray();
                byte[] desIV = this.keytoByteArray();

                //Create the file streams to handle the input and output files.
                FileStream fin = new FileStream(inName, FileMode.Open, FileAccess.Read);
                FileStream fout = new FileStream(outName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);

                //Create variables to help with read and write. 
                byte[] bin = new byte[100];     //This is intermediate storage for the encryption. 
                long rdlen = 0;                 //This is the total number of bytes written. 
                long totlen = fin.Length;       //This is the total length of the input file. 
                int len;                        //This is the number of bytes to be written at a time.

                DES des = new DESCryptoServiceProvider();
                CryptoStream decStream = new CryptoStream(fout, des.CreateDecryptor(desKey, desIV), CryptoStreamMode.Write);

                Console.WriteLine("Decrypting...");

                //Read from the input file, then encrypt and write to the output file. 
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 100);
                    decStream.Write(bin, 0, len);
                    rdlen = rdlen + len;
                    Console.WriteLine("{0} bytes processed", rdlen);
                }

                Console.WriteLine("Decryption complete.");
                decStream.Close();
                fout.Close();
                fin.Close();   
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
            }
        }

        private byte[] keytoByteArray()
        {
            /*
             * Keys are 64 bits which are represented by an 8 byte array.
             * The key is formed from the password string by taking the 
             * low order 8 bits of each Unicode character (cast to a byte)
             * and storing it in the byte array.
             * If the string is more than 8 characters the 9th character's
             * 8 bit value is added to the first byte in the array, the 10th character
             * is added to the second byte etc.
            */

            // The array is initialized to 0
            // byte[] KeyArray = new byte[8] {0,0,0,0,0,0,0,0};
            byte[] KeyArray = Enumerable.Repeat((byte)0, 8).ToArray();
            
            for (int i = 0; i < KeyText.Length; i++)
            {
                byte b = (byte)KeyText[i];
                KeyArray[i % 8] = (byte)(KeyArray[i % 8] + b);
            }

            return KeyArray;
        }

    }
}
