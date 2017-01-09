using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Tube{
	public Vector3 start; 
	public Vector3 end;
	public int id;
	public float r;
	public int prev_id = -1;
	public int base_id;
	public int top_id;
	public bool first;
	public bool fixed_blood_pressure;
	public float pressure;
	public int m_id;

	internal Tube(int ida, Vector3 starta, Vector3 enda, float ra, int prev_ida , int base_ida = -1, int top_ida = -1, bool firsta = false, bool fixed_blood_pressurea = false, float pressurea = 0.0f){
		id = ida;
		prev_id= prev_ida;
		start = starta;
		end = enda;
		r = ra;
		base_id=base_ida;
		top_id = top_ida;
		first = firsta;
		fixed_blood_pressure= fixed_blood_pressurea;
		pressure = pressurea;
	}

	public void set_id(int m_ida){
		m_id = m_ida;
	}

	public void switch_ends(){
		Vector3 tmp = start;
		start = end;
		end = tmp;
	}

	public void switch_to_top_id(){
		if( top_id == -1){
			top_id = base_id;
			base_id = -1;
		}
	}
}