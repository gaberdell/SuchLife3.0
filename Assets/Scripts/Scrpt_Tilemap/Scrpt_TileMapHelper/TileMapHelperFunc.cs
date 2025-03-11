using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileMapHelper
{
    public class TileMapHelperFunc
    {
        

        public Vector3 isTilePartOfRay(Tilemap currentTileMap, Vector3 origin, Vector3 directionOfVector, float stepSize, out Vector3 vector3WithNoBlock, out List<Vector3> listOfStepPoints, out bool hitBlock)
        {
            listOfStepPoints = new List<Vector3>();

            float sizeOfVector = directionOfVector.magnitude;

            Vector3 stepAddVector = directionOfVector.normalized * stepSize;
            Vector3 previousStepThrough = origin;
            bool successfulReachedEnd = true;
            hitBlock = false;

            listOfStepPoints.Clear();


            Vector3 stepThroughVector;
            for (stepThroughVector = origin; (stepThroughVector - origin).magnitude < sizeOfVector; stepThroughVector += stepAddVector)
            {
                if ((currentTileMap.GetTile(currentTileMap.WorldToCell(stepThroughVector))) != null)
                {
                    successfulReachedEnd = false;
                    hitBlock = true;
                    break;

                }

                listOfStepPoints.Add(previousStepThrough);
                previousStepThrough = stepThroughVector;
            }

            if (successfulReachedEnd == true)
            {
                Vector3 totalVector = origin + directionOfVector;
                if (currentTileMap.GetTile(currentTileMap.WorldToCell(totalVector)) == null)
                {
                    listOfStepPoints.Add(previousStepThrough);
                    previousStepThrough = totalVector;
                }
                else
                {
                    hitBlock = true;
                }
            }

            if (previousStepThrough != origin)
            {
                vector3WithNoBlock = previousStepThrough;
                return stepThroughVector;
            }
            else
            {
                vector3WithNoBlock = origin;
                return stepThroughVector;
            }

        }
    
        public Vector3 isTilePartOfRay(Tilemap[] tileMapsToCheck, Vector3 origin, Vector3 directionOfVector, float stepSize, out Vector3 vector3WithNoBlock, out List<Vector3> listOfStepPoints, out bool hitBlock)
        {
            hitBlock = false;
            listOfStepPoints = new List<Vector3>();
            vector3WithNoBlock = origin;

            Vector3 currentHitPoint = origin;

            if ((Vector2) directionOfVector == Vector2.zero)
            {
                return origin;
            }

 
            foreach (Tilemap currentTileMap in tileMapsToCheck)
            {
                currentHitPoint = isTilePartOfRay(currentTileMap, origin, directionOfVector, stepSize, out vector3WithNoBlock, out listOfStepPoints, out hitBlock);
                if (hitBlock)
                {
                    return currentHitPoint;
                }
            }

            return currentHitPoint;
        }
    }
}
