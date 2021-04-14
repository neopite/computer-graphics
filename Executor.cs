﻿using System;
using System.Linq;
using ImageConverter.Rendering.Renderer;
using ImageConverter.Writers;
using ImageFormatConverter;

namespace ImageConverter
{
    class Executor
    {
        public static void Main(string[] args)
        {
            IRenderer rendering = new Rendering.Rendering();
            new BmpWriter("D:\\Study\\CompAssignment\\ComputerGraphics\\Images").WriteImage(rendering.RenderObj("D:\\Study\\CompAssignment\\ComputerGraphics\\Images\\cow.obj"));
            
            /*Console.WriteLine(args.Length);
            string source = args[0].Substring(9);
            string format = args[1].Substring(14);
            string output = args.Length > 2 ? args[2].Substring(9) : source.Split('.')[0];
            
            if (Enum.GetNames(typeof(ImageWriteFormat)).ToList().Contains(format.ToUpper()))
            {
                ImageConverter image = new ImageConverter(source, format, output);
                image.ConvertImage();
            }
            else throw new OutputFormatNotExistedException(format);*/
            
        }
    }
}