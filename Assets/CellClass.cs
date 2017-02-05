using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Cell{
	public Vector3 position;
	public string tissue;

	internal Cell(Vector3 pos, string col){
		position = pos;
		tissue = col;
	}
}