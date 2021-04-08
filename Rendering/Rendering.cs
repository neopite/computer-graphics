﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageConverter.ImageStructure;
using ObjLoader.Loader.Loaders;

namespace ImageConverter.Rendering
{
    public class Rendering
    {
        private static readonly Color _blackPixel = new Color(0, 0, 0);
        private static readonly Color _redPixel = new Color(255, 0, 0);
        
        public Image Render()
        {
            #region Consts
            Vector3 cameraPosition = new Vector3(0, 0, -1);
            Vector3 centerScreen = new Vector3(0, 0, 0);
            Vector3 camLookDirection = (centerScreen - cameraPosition).Normalize();
            double fov = 90;
            double distanceToPlaneFromCamera = (cameraPosition - centerScreen).Length;
            #endregion

            
            #region Shapes
            Triangle triangle = new Triangle(new Vector3(-0.5, -0.5, 5)
                , new Vector3(0, 0.5, 5)
                , new Vector3(0.5, -0.5, 5));
            Triangle triangle1 = new Triangle(new Vector3(3, 3, 5)
                , new Vector3(5, 1, 5)
                , new Vector3(4, 4, 5));
            List<Triangle> listOfTriangles = new List<Triangle>();
            listOfTriangles.Add(triangle);
            listOfTriangles.Add(triangle1);
            #endregion

            List<Triangle> bluadCow = ParseObj();
            
            double screenSize = GetScreenSize(distanceToPlaneFromCamera,fov);
            Image image = new Image(500,500);
            ImagePalette imagePalette = new ImagePalette();
            List<Vector3> arrayOfPixelsCenters = GetScreenPointsForRay(centerScreen,screenSize,image);
            Vector3[,] rays = GetRays(cameraPosition,arrayOfPixelsCenters,image);
            for (int i = 0; i < rays.GetLength(0); i++)
            {
                for (int j = 0; j < rays.GetLength(1); j++)
                {
                    bool isFilled = false;
                    foreach (Triangle tr in bluadCow)
                    {
                        isFilled = MollerTrumbore.RayIntersectsTriangle(cameraPosition, rays[i, j], tr);
                        if(isFilled) break;
                    }
                    if(isFilled) imagePalette.ListOfPixels.Add(new Pixel(i, j, _redPixel));
                    else imagePalette.ListOfPixels.Add(new Pixel(i, j, _blackPixel));
                    isFilled = false;
                }
            }

            image.ImagePalette = imagePalette;
            return image;
        }
        
        //Assume tha    t width == height
        private double GetScreenSize(double distanceFromCamToScreen , double fov)
        {
            double rad = DegreeToRad(fov);
            double size = 2 * distanceFromCamToScreen * Math.Tan(rad/2);
            return size;
        }

        private static double DegreeToRad(double degree)
        {
            return  degree / 180f * Math.PI;
        }

        /*private Vector3 GetLowerLeftAngle(double screenSize,Vector3 screenCenter)
        {
            return screenCenter - new Vector3(screenSize/2,screenSize/2,0);
        }*/

        private List<Vector3> GetScreenPointsForRay(Vector3 screenCenter, double  screenSize , Image goalImage)
        {
            List<Vector3> listPointsForRay = new List<Vector3>();
            
            int imageHeight = goalImage.Height;
            int imageWidth = goalImage.Width;

            double pixHeight = screenSize / imageHeight;
            double pixWidth = screenSize / imageWidth;
            
            double pZ = screenCenter.z; // const as for surface ( parallel oxy )
            
            for (int i = 0; i < imageHeight; i++)       
            {
                double pY = screenCenter.y - screenSize / 2 + pixHeight * (i + 0.5);
                for (int j = 0; j < imageWidth; j++)
                {
                    double pX = screenCenter.x - screenSize / 2 + pixWidth * (j + 0.5);         //pZ const ( takes from center coord)
                    Vector3 screenPointForRay = new Vector3(pX, pY, pZ);
                    listPointsForRay.Add(screenPointForRay);
                }
            }
            return listPointsForRay;
        }

        private Vector3[,] GetRays(Vector3 originCamera , List<Vector3> listOfCentersOnScreen , Image image)
        {
            Vector3[,] screenRays = new Vector3[image.Width,image.Height];
            for (int i = 0; i < screenRays.GetLength(0); i++)
            {
                for (int j = 0; j < screenRays.GetLength(1); j++)
                {
                    screenRays[i, j] = (listOfCentersOnScreen[(i * screenRays.GetLength(1)) + j] - originCamera).Normalize();
                }
            }
            return screenRays;
        }

        private List<Triangle> ParseObj()
        {
            var objLoaderFactory = new ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create();
            var fileStream = new FileStream("D:\\Study\\CompGraphics\\ComputerGraphics\\Images\\cow.obj",FileMode.Open);
            var result = objLoader.Load(fileStream);
            List<Triangle> cow = new List<Triangle>();
            for (int i = 0; i < result.Groups.Count; i++)
            {
                for (int j = 0; j < result.Groups[i].Faces.Count; j++)
                {
                    Vector3 a = new Vector3(result.Vertices[result.Groups[i].Faces[j][0].VertexIndex - 1].X,
                        result.Vertices[result.Groups[i].Faces[j][0].VertexIndex - 1].Y,
                        result.Vertices[result.Groups[i].Faces[j][0].VertexIndex - 1].Z);
                    
                    Vector3 b =new Vector3(result.Vertices[result.Groups[i].Faces[j][1].VertexIndex - 1].X,
                        result.Vertices[result.Groups[i].Faces[j][1].VertexIndex - 1].Y,
                        result.Vertices[result.Groups[i].Faces[j][1].VertexIndex - 1].Z);

                    Vector3 c =new Vector3(result.Vertices[result.Groups[i].Faces[j][2].VertexIndex - 1].X,
                        result.Vertices[result.Groups[i].Faces[j][2].VertexIndex - 1].Y,
                        result.Vertices[result.Groups[i].Faces[j][2].VertexIndex - 1].Z);
                    
                    cow.Add(new Triangle(a,b,c));
                }
            }

            return cow;
        }
        
    }
}
