﻿using System;
using System.Collections.Generic;
using System.Linq;
using ImageConverter.ImageStructure;

namespace ImageConverter.Rendering
{
    public class Rendering
    {
        public void Render()
        {
            #region Consts
            Vector3 cameraPosition = new Vector3(0, 0, 0);
            Vector3 centerScreen = new Vector3(0, 0, 1);
            Vector3 camLookDirection = (centerScreen - cameraPosition).Normalize();
            double fov = 60;
            double distanceToPlaneFromCamera = (cameraPosition - centerScreen).Length;
           // double realPlaneHeight = (double) (distanceToPlaneFromCamera * Math.Tan(fovInRad)); // height == width
            #endregion

            #region Shapes
            Triangle triangle = new Triangle(new Vector3(0, 5f, -1)
                , new Vector3(5f, -5f, -1)
                , new Vector3(-5f, -5f, -1));
            #endregion
    
            double screenSize = GetScreenSize(distanceToPlaneFromCamera,fov);
            /*
            Vector3 lowerLeftAnglePos = GetLowerLeftAngle(screenSize,centerScreen);
            */
            Image image = new Image(50,50);
            List<Vector3> arrayOfPixelsCenters = GetScreenPointsForRay(centerScreen,screenSize,image);
            List<Vector3> getRays = GetRays(cameraPosition,arrayOfPixelsCenters);
        }
        
        //Assume that width == height
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
                    Vector3 screenPointForRay = new Vector3(pX, pY, pZ).Normalize();
                    listPointsForRay.Add(screenPointForRay);
                }
            }

            return listPointsForRay;
        }

        private List<Vector3> GetRays(Vector3 originCamera , List<Vector3> listOfCentersOnScreen)
        {
            return listOfCentersOnScreen.Select(point => point - originCamera).ToList();
        }
    }
}
