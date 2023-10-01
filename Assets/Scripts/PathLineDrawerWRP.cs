using UnityEngine;
using System.Collections;

public class PathLineDrawerWRP : MonoBehaviour {

/*
*	FUNCTION:
*	- This script is responsible for drawing the spline visually in the editor.
*	
*	USED BY: 
*	This script is part of the CP_Straight and CP_Curve prefab which are used with
*	environment patches.
*
*/

public Vector3[] Parameterized_CPPositions = new Vector3[53];
public float fPathLength = 3000.0f;	//default length displacement of each patch



void OnDrawGizmos (){
	if(transform.Find("CP_01"))
	{
		Transform ParentGroup = transform;
		int i = 0;
		foreach(Transform child in ParentGroup)
			i++;
		int NumCPs = i;
		Vector3[] vectorsArray = new Vector3[NumCPs];
		for(i=0;i<NumCPs;i++)
		{
			if(i<9)
				vectorsArray[i] = ParentGroup.Find("CP_0"+(i+1)).position;
			else
				vectorsArray[i] = ParentGroup.Find("CP_"+(i+1)).position;
		}
	    iTween.DrawPath(vectorsArray);
    }
}

public void SetCPValues (){
	SetCurrentPatchCPs();
}

private void SetCurrentPatchCPs (){
	int i = 0;
	
	Vector3[] tempCPPositions;
	Transform[] AllCPs;
//	Parameterized_CPPositions = [];
	
	Transform CPs_Group = transform;
	
	foreach(Transform child in CPs_Group)
		i++;
	int NumCPs = i;
	
	i = 0;
	
	AllCPs = new Transform[NumCPs];
	tempCPPositions = new Vector3[NumCPs];
	
	i = 0;
	foreach(Transform child in CPs_Group)
	{
		AllCPs[i] = child;
		i++;
	}
	
	AllCPs = SortCPsbyName(AllCPs, 0,NumCPs);
	
	for(i=0;i<NumCPs;i++)
	{
		tempCPPositions[i] = AllCPs[i].position;
		tempCPPositions[i].y = 0;
	}	
	
	Parameterized_CPPositions = PathControlPointGenerator(tempCPPositions);
	Parameterized_CPPositions = ParameterizeCPs(Parameterized_CPPositions);
	
	fPathLength = PathLength(Parameterized_CPPositions);
}

private Vector3[] ParameterizeCPs ( Vector3[] pts  ){
	float i = 0.0f;
	float Current_TD = 0.0f;	 //Current total distance
	float TotalPathLength = PathLength(pts);
	
	float CP_Increment = TotalPathLength/50.0f;
	Vector3 PreviousPoint = pts[1];
	Vector3 CurrentPoint = PreviousPoint;
	Vector3[] FinalPoints = new Vector3[51];
	int Index = 0;
	FinalPoints[Index] = pts[1];
	Index++;
	for(i=0;i<=1.0f;i+=0.000001f)
	{
		CurrentPoint = Interp(pts,i);
		Current_TD+=Vector3.Distance(CurrentPoint,PreviousPoint);
		if(Current_TD>=CP_Increment)
		{
			FinalPoints[Index] = CurrentPoint;
			Current_TD = 0;
			Index++;
		}
		PreviousPoint = CurrentPoint;
	}
        FinalPoints[50] = pts[pts.Length-2];
	FinalPoints = PathControlPointGenerator(FinalPoints);

	return FinalPoints;
}

private Vector3[] PathControlPointGenerator ( Vector3[] path  ){
		Vector3[] suppliedPath;
		Vector3[] vector3s;
		
		//create and store path points:
		suppliedPath = path;

		//populate calculate path;
		int offset  = 2;
		vector3s = new Vector3[suppliedPath.Length+offset];
		System.Array.Copy(suppliedPath,0,vector3s,1,suppliedPath.Length);
		
		//populate start and end control points:
		vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
		vector3s[vector3s.Length-1] = vector3s[vector3s.Length-2] + (vector3s[vector3s.Length-2] - vector3s[vector3s.Length-3]);
		
		//is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
		if(vector3s[1] == vector3s[vector3s.Length-2]){
			Vector3[] tmpLoopSpline  = new Vector3[vector3s.Length];
			System.Array.Copy(vector3s,tmpLoopSpline,vector3s.Length);
			tmpLoopSpline[0]=tmpLoopSpline[tmpLoopSpline.Length-3];
			tmpLoopSpline[tmpLoopSpline.Length-1]=tmpLoopSpline[2];
			vector3s=new Vector3[tmpLoopSpline.Length];
			System.Array.Copy(tmpLoopSpline,vector3s,tmpLoopSpline.Length);
		}
		
		return(vector3s);
}
	
//andeeee from the Unity forum's steller Catmull-Rom class ( http://forum.unity3d.com/viewtopic.php?p=218400#218400 ):
private Vector3 Interp ( Vector3[] pts  ,   float t  ){
		t = Mathf.Clamp(t,0.0f,2.0f);
		//t = ActualPercentage(t);
		int numSections  = pts.Length - 3;
        int currPt  = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
        float u = t * (float)numSections - (float)currPt;
		Vector3 a = pts[currPt];
		Vector3 b = pts[currPt + 1];
		Vector3 c = pts[currPt + 2];
		Vector3 d = pts[currPt + 3];
		
		return .5f * (
			(-a + 3f * b - 3f * c + d) * (u * u * u)
			+ (2f * a - 5f * b + 4f * c - d) * (u * u)
			+ (-a + c) * u
			+ 2f * b
		);
}

private float PathLength ( Vector3[] pathPoints  ){
		float pathLength  = 0;
		
		Vector3[] vector3s  = pathPoints;
		
		//Line Draw:
		Vector3 prevPt  = Interp(vector3s,0);
		int SmoothAmount  = pathPoints.Length*20;
		for (int i = 1; i <= SmoothAmount; i++)
		{
            float pm = (float) i / SmoothAmount;
			Vector3 currPt  = Interp(vector3s,pm);
			pathLength += Vector3.Distance(prevPt,currPt);
			prevPt = currPt;
		}
		
		return pathLength;
}

private Transform[] SortCPsbyName ( Transform[] CPs ,   int startIndex ,   int endIndex  ){
        ArrayList names = new ArrayList();
	//FIXME_VAR_TYPE final= new Array();
    
    Transform[] tempCPs = new Transform[endIndex-startIndex];
    int j = 0;
    int i = 0;
    for(i=startIndex;i<endIndex;i++)
    {
    	tempCPs[j] = CPs[i];
    	j++;
    }
    
	foreach(Transform go in tempCPs)
            names.Add(go.name);
	
        names.Sort();
	
	i=startIndex;
	foreach(string name in names)
	{
		foreach(Transform go in tempCPs)
		{
			if(go.name==name)
			{
				CPs[i] = go;
				break;
			}
    	}
    	i++;
    }
	return CPs;
}

}