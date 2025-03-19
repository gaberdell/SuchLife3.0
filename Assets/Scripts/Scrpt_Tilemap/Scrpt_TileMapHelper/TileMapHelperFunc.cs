using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileMapHelper
{


    public class TileMapHelperFunc
    {

        //javi video was very helpful for this https://www.youtube.com/watch?v=NbSee-XM7WA
        public static Vector3 DDARayCheck(Tilemap currentTileMap, Vector3 origin, Vector3 directionOfVector, float stepSize, out Vector3 vector3WithNoBlock, out List<Vector3> listOfStepPoints, out bool hitBlock)
        {
            //List of points that we can store!
            listOfStepPoints = new List<Vector3>();

            Vector2 difVec = origin - directionOfVector;
            Vector2 difNorm = difVec.normalized;


            float rayDifYX = (difNorm.y / difNorm.x);
            float rayDifXY = (difNorm.x / difNorm.y);

            //Essentially we figure out how much the magnitude increases per each unit of one
            Vector2 vecRayUnitStepSize = new Vector2(Mathf.Sqrt(1 + rayDifYX*rayDifYX), Mathf.Sqrt(1 + rayDifXY * rayDifXY));

            Vector3Int mapCheck = currentTileMap.WorldToCell(origin);

            Vector2 rayLength1D = Vector2Int.zero;

            Vector2Int blockStep = Vector2Int.zero;

            if (difNorm.x < 0)
            {
                blockStep.x = -1;
                rayLength1D.x = (origin.x - (float)mapCheck.x) * vecRayUnitStepSize.x;
            }
            else
            {
                blockStep.x = 1;
                rayLength1D.x = ((float)(mapCheck.x + 1) - origin.x) * vecRayUnitStepSize.x;
            }

            if (difNorm.y < 0)
            {
                blockStep.y = -1;
                rayLength1D.y = (origin.y - (float)mapCheck.y) * vecRayUnitStepSize.y;
            }
            else
            {
                blockStep.y = 1;
                rayLength1D.y = ((float)(mapCheck.x + 1) - origin.y) * vecRayUnitStepSize.y;
            }

            vector3WithNoBlock = origin;

            hitBlock = false;
            float currentDistance = 0f;
            float lastFailedDist = 0f;
            float maxDistance = difVec.magnitude;
            while (!hitBlock && currentDistance < maxDistance)
            {
                if (rayLength1D.x < rayLength1D.y)
                {
                    mapCheck.x += blockStep.x;
                    currentDistance = rayLength1D.x;
                    rayLength1D.x += vecRayUnitStepSize.x;
                }
                else
                {
                    mapCheck.y += blockStep.y;
                    currentDistance = rayLength1D.y;
                    rayLength1D.y += vecRayUnitStepSize.y;
                }

                if (currentTileMap.GetTile(mapCheck) == null)
                {
                    lastFailedDist = currentDistance;
                    listOfStepPoints.Add(rayLength1D);
                }
                else
                {
                    hitBlock = true;
                }
            }
            vector3WithNoBlock = origin + (Vector3)(difNorm * lastFailedDist);
            return origin + (Vector3)(difNorm * currentDistance);
        }

        public static Vector3 isTilePartOfRay(Tilemap currentTileMap, Vector3 origin, Vector3 directionOfVector, float stepSize, out Vector3 vector3WithNoBlock, out List<Vector3> listOfStepPoints, out bool hitBlock)
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
    
        public static Vector3 isTilePartOfRay(Tilemap[] tileMapsToCheck, Vector3 origin, Vector3 directionOfVector, float stepSize, out Vector3 vector3WithNoBlock, out List<Vector3> listOfStepPoints, out bool hitBlock)
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
