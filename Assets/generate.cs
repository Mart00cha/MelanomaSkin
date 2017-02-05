﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class generate : MonoBehaviour {

	public GameObject cylinderPrefab;
	public GameObject cellPrefab;
	public GameObject vessel;
	public GameObject layer1;

	//these variables should be set up in the menu
	public float pairs_of_vessels = 2.0f;
	public float ups_per_vessel = 3.0f;
	public float sector_length = 0.5f;
	public float vessel_length = 30.0f;
	public float artery_vein_dist = 3.0f;
	public float pairs_dist = 5.0f;
	public float max_thickness = 0.5f;
	public float min_thickness = 0.05f;
	public float levels = 20.0f;
	public float chance_of_split = 0.05f;
	public float max_distance_of_split = 1.5f; 
	public float max_curvature = 0.3f;
	public float cell_size = 0.3f;
	public float cell_curv = 0.0f;



	private bool params_changed = false;
	private int id;

	public float size_x;
	public float size_y;
	public float size_z;
	


	public System.Collections.Generic.List<Tube> export_dict;
	public System.Collections.Generic.List<Cell> export_cells;

	public void AdjustCellSize(float val){
		cell_size = val; 
	}

	public void AdjustCellCurv(float val){
		cell_curv = val; 
	}

	public void AdjustPairs(float val){
		pairs_of_vessels = val; 
		params_changed = true;
	}

	public void AdjustUps(float val){
		ups_per_vessel = val; 
		params_changed = true;
	}

	public void AdjustSecLen(float val){
		sector_length = val; 
		params_changed = true;
	}

	public void AdjustVesLen(float val){
		vessel_length = val; 
		params_changed = true;
	}

	public void AdjustArtery(float val){
		artery_vein_dist = val; 
		params_changed = true;
	}

	public void AdjustPairsDist(float val){
		pairs_dist = val; 
		params_changed = true;
	}


	public void AdjustMaxThick(float val){
		max_thickness = val; 
		params_changed = true;
	}

	public void AdjustLevels(float val){
		levels = val; 
		params_changed = true;
	}

	public void AdjustChance(float val){
		chance_of_split = val; 
		params_changed = true;
	}

	public void AdjustMaxDist(float val){
		max_distance_of_split = val; 
		params_changed = true;
	}

	public void AdjustMaxCurv(float val){
		max_curvature = val; 
		params_changed = true;
	}

	void Start(){
		vessel = new GameObject("Vessel");
		Generate();
	}




	public void GenerateCells (){
		
		

		export_cells = new List<Cell>();

		float color;

		GameObject.Destroy(layer1);
		layer1 = new GameObject("Layer1");
		float scale = cell_size;

		float count_x = size_x/cell_size;
		float count_y = size_y/cell_size;
		float count_z = size_z/cell_size;


		for(float x=0.0f; x<=count_x; x+=1){
			for(float z = 0.0f; z<= count_z; z+=1){
				for(float y = 0.0f; y<= count_y; y+=1){
					Vector3 position = new Vector3((x-2.0f)*cell_size, (y-1.0f)*cell_size, z*cell_size + pairs_dist);
					GameObject sphere = Instantiate (cellPrefab, position, Quaternion.identity) as GameObject;
			        sphere.transform.localScale = Vector3.one * scale;
					sphere.GetComponent<Renderer> ().material = new Material(Shader.Find("Transparent/Diffuse"));
					color = ((Mathf.Sin(x) + Mathf.Sin(z)) * cell_curv) + y*cell_size;
					if(color > count_y * 0.9f){
						sphere.GetComponent<Renderer> ().material.color = new Color(0.4f,0.4f,0.4f,0.1f);
						export_cells.Add(new Cell(position, "epidermis"));
					}else if (color > count_y * 0.1f){
							sphere.GetComponent<Renderer> ().material.color = new Color(0.0f,1.0f,0.0f,0.1f);
							export_cells.Add(new Cell(position, "dermis"));
					}else {
						sphere.GetComponent<Renderer> ().material.color = new Color(0.0f,0.0f,1.0f,0.1f);
						export_cells.Add(new Cell(position, "hypodermis"));
					}
			        sphere.transform.parent = layer1.transform;
				}
			}
		}
		
	}

	// Use this for initialization
	void Generate () {
		params_changed = false;
		GameObject.Destroy(vessel);
		vessel = new GameObject("Vessel");

		size_x = vessel_length + 5.0f;
		size_y = (levels * 1.3f) * sector_length;
		size_z = (pairs_of_vessels + 1.0f) * pairs_dist;

		//computed values
		float grid_sectors_per_vessel = vessel_length / sector_length;
		System.Collections.Generic.Queue<Vector3> qv = new Queue<Vector3>(); //keeps track of id's of not ended Tubes
		System.Collections.Generic.Queue<Vector3> qa = new Queue<Vector3>(); //keeps track of id's of not ended Tubes
		Vector3 split_point;
		Vector3 end_split_point;
		Tube split_tube;
		float noise;
		float q_size;
		Vector3 point;
		Vector3 endpoint1;
		Vector3 endpoint2;
		Vector3 connection_point;
		System.Collections.Generic.List<Tube> base_qv = new List<Tube>();
		System.Collections.Generic.List<Tube> base_qa = new List<Tube>();
		export_dict = new List<Tube>();
		id = 0;

		//create base vessels
		for(float i=1.0f; i<= pairs_of_vessels; i++){
			base_qv = CreateLongCylinderWithReturnPoints(new Vector3(0,0, i*(pairs_dist+artery_vein_dist)), new Vector3(vessel_length, 0, i*(pairs_dist+artery_vein_dist)), max_thickness, sector_length);
			base_qa = CreateLongCylinderWithReturnPoints(new Vector3(0,0, i*(pairs_dist+artery_vein_dist)+artery_vein_dist), new Vector3(vessel_length,0, i*(pairs_dist+artery_vein_dist)+artery_vein_dist), max_thickness, sector_length);
			export_dict.AddRange(base_qv);
			export_dict.AddRange(base_qa);
			var rnd = new System.Random();
			var numbers = new List<int>(Enumerable.Range(0, base_qv.Count).OrderBy(item => rnd.Next()));


			for(int k=1; k<= ups_per_vessel; k++){
				split_tube = base_qv[numbers[k]];
				split_point = new Vector3((split_tube.end.x + split_tube.start.x)/2.0f, (split_tube.end.y + split_tube.start.y)/2.0f, (split_tube.end.z + split_tube.start.z)/2.0f);
				end_split_point = new Vector3(split_point.x,split_point.y + sector_length, split_point.z);
				CreateCylinderBetweenPoints(split_point, end_split_point, max_thickness/2.0f);
				export_dict.Add(new Tube(id, split_point, end_split_point, max_thickness/2.0f, -1, split_tube.id, -1, true));
				id +=1;
				qv.Enqueue(end_split_point);

				split_tube = base_qa[numbers[k]];
				split_point = new Vector3((split_tube.end.x + split_tube.start.x)/2.0f, (split_tube.end.y + split_tube.start.y)/2.0f, (split_tube.end.z + split_tube.start.z)/2.0f);
				end_split_point = new Vector3(split_point.x,split_point.y + sector_length, split_point.z);
				CreateCylinderBetweenPoints(split_point, end_split_point, max_thickness/2.0f);
				export_dict.Add(new Tube(id, split_point, end_split_point, max_thickness/2.0f, -1, split_tube.id, -1, true));
				id +=1;
				qa.Enqueue(end_split_point);
			}		
		}


		//create veins going up
		for(float lvl = 1.0f; lvl<levels; lvl++){
			q_size = qv.Count;
			for(float v=1.0f; v<=q_size; v++){
				point = qv.Dequeue();
				split_tube = Find_by_endpoint(point); 
				noise = Random.Range(0.0f, 1.0f);
				if(noise < chance_of_split){
					endpoint1 = new Vector3(point.x + Random.Range(-max_distance_of_split,max_distance_of_split), point.y + sector_length, point.z + Random.Range(-1.0f *max_distance_of_split,max_distance_of_split));
					endpoint2 = new Vector3(point.x + Random.Range(-max_distance_of_split,max_distance_of_split), point.y + sector_length, point.z + Random.Range(-1.0f * max_distance_of_split,max_distance_of_split));
					qv.Enqueue(endpoint1);
					qv.Enqueue(endpoint2);
					CreateCylinderBetweenPoints(point, endpoint1, ((levels-lvl)*((max_thickness-min_thickness)/levels))/2.0f);
					export_dict.Add(new Tube(id, point, endpoint1, ((levels-lvl)*((max_thickness-min_thickness)/levels))/2.0f, split_tube.id));
					id +=1;
					split_point = new Vector3((endpoint1.x + point.x)/2.0f, (endpoint1.y + point.y)/2.0f, (endpoint1.z + point.z)/2.0f);
					CreateCylinderBetweenPoints(split_point, endpoint2, ((levels -lvl)*((max_thickness-min_thickness)/levels))/2.0f); 
					export_dict.Add(new Tube(id, split_point, endpoint2, ((levels -lvl)*((max_thickness-min_thickness)/levels))/2.0f,-1, id - 1 , -1, true));
					id +=1;
				} else {
					endpoint1 = new Vector3(point.x + Random.Range(-max_curvature,max_curvature), point.y + sector_length, point.z + Random.Range(-1.0f * max_curvature,max_curvature));
					qv.Enqueue(endpoint1);
					CreateCylinderBetweenPoints(point, endpoint1, ((levels -lvl)*((max_thickness-min_thickness)/levels))/2.0f);
					export_dict.Add(new Tube(id, point, endpoint1, ((levels-lvl)*((max_thickness-min_thickness)/levels))/2.0f, split_tube.id));
					id +=1;
				}
			}
		}

		//create arteries going up
		for(float lvl = 1.0f; lvl<levels; lvl++){
			q_size = qa.Count;
			for(float v=1.0f; v<=q_size; v++){
				point = qa.Dequeue();
				split_tube = Find_by_endpoint(point); 
				noise = Random.Range(0.0f, 1.0f);
				if(noise < chance_of_split){
					endpoint1 = new Vector3(point.x + Random.Range(-max_distance_of_split,max_distance_of_split), point.y + sector_length, point.z + Random.Range(-1.0f *max_distance_of_split,max_distance_of_split));
					endpoint2 = new Vector3(point.x + Random.Range(-max_distance_of_split,max_distance_of_split), point.y + sector_length, point.z + Random.Range(-1.0f * max_distance_of_split,max_distance_of_split));
					qa.Enqueue(endpoint1);
					qa.Enqueue(endpoint2);
					CreateCylinderBetweenPoints(point, endpoint1, ((levels -lvl)*((max_thickness-min_thickness)/levels))/2.0f);
					export_dict.Add(new Tube(id, point, endpoint1, ((levels-lvl)*((max_thickness-min_thickness)/levels))/2.0f, split_tube.id));
					id +=1;
					split_point = new Vector3((endpoint1.x + point.x)/2.0f, (endpoint1.y + point.y)/2.0f, (endpoint1.z + point.z)/2.0f);
					CreateCylinderBetweenPoints(split_point, endpoint2, ((levels-lvl)*((max_thickness-min_thickness)/levels))/2.0f);
					export_dict.Add(new Tube(id, split_point, endpoint2, ((levels-lvl)*((max_thickness-min_thickness)/levels))/2.0f, -1, id - 1 , -1, true));
					id +=1;
				} else {
					endpoint1 = new Vector3(point.x + Random.Range(-max_curvature,max_curvature), point.y + sector_length, point.z + Random.Range(-1.0f * max_curvature,max_curvature));
					qa.Enqueue(endpoint1);
					CreateCylinderBetweenPoints(point, endpoint1, ((levels-lvl)*((max_thickness-min_thickness)/levels))/2.0f);
					export_dict.Add(new Tube(id, point, endpoint1, ((levels-lvl)*((max_thickness-min_thickness)/levels))/2.0f, split_tube.id));
					id +=1;
				}
			}
		}

		qa = new Queue<Vector3>(qa.OrderBy( vec => vec.z+ vec.x*3.0f));
		qv = new Queue<Vector3>(qv.OrderBy( vec => vec.z+ vec.x*3.0f));


		//zniweluj różnicę pomiędzy ilością zakończeń

		int diff = qv.Count - qa.Count;

		Tube tube;

		if(diff >0) {
			while(diff >0){
				endpoint1 = qv.Dequeue();
				endpoint2 = qv.Dequeue();
				connection_point = new Vector3((endpoint1.x + endpoint2.x)/2.0f, (endpoint1.y + endpoint2.y)/2.0f, (endpoint1.z + endpoint2.z)/2.0f);
				tube = CreateLongCylinderWithReturn(endpoint1, connection_point, min_thickness/2.0f, sector_length, Find_by_endpoint(endpoint1).id);
				CreateLongCylinderWithMove(endpoint2, tube, min_thickness/2.0f, sector_length, Find_by_endpoint(endpoint2).id, Find_by_endpoint(tube.end).id);
				qv.Enqueue(connection_point);
				diff -=1;
			}
		} else {
			while(diff <0){
				endpoint1 = qa.Dequeue();
				endpoint2 = qa.Dequeue();
				connection_point = new Vector3((endpoint1.x + endpoint2.x)/2.0f, (endpoint1.y + endpoint2.y)/2.0f, (endpoint1.z + endpoint2.z)/2.0f);
				tube = CreateLongCylinderWithReturn(endpoint1, connection_point, min_thickness/2.0f, sector_length, Find_by_endpoint(endpoint1).id);
				CreateLongCylinderWithMove(endpoint2, tube, min_thickness/2.0f, sector_length, Find_by_endpoint(endpoint2).id, Find_by_endpoint(tube.end).id);
				qa.Enqueue(connection_point);
				diff +=1;
			}
		}



		// //lower number of connections
		// //tutaj jest cos zrypane
		// float lower_by = Mathf.Floor(qv.Count/2.0f);

		// while(lower_by>0){
		// 	endpoint1 = qv.Dequeue();
		// 	endpoint2 = qv.Dequeue();
		// 	connection_point = new Vector3((endpoint1.x + endpoint2.x)/2.0f, (endpoint1.y + endpoint2.y)/2.0f, (endpoint1.z + endpoint2.z)/2.0f);
		// 	CreateLongCylinder(endpoint1, connection_point, max_thickness/(1 + Mathf.Floor(levels*0.2f)), sector_length, Find_by_endpoint(endpoint1).id);
		// 	CreateLongCylinder(endpoint2, connection_point, max_thickness/(1 + Mathf.Floor(levels*0.2f)), sector_length, Find_by_endpoint(endpoint2).id);
		// 	qv.Enqueue(connection_point);

		// 	endpoint1 = qa.Dequeue();
		// 	endpoint2 = qa.Dequeue();
		// 	connection_point = new Vector3((endpoint1.x + endpoint2.x)/2.0f, (endpoint1.y + endpoint2.y)/2.0f, (endpoint1.z + endpoint2.z)/2.0f);
		// 	CreateLongCylinder(endpoint1, connection_point, max_thickness/(1 + Mathf.Floor(levels*0.2f)), sector_length, Find_by_endpoint(endpoint1).id);
		// 	CreateLongCylinder(endpoint2, connection_point, max_thickness/(1 + Mathf.Floor(levels*0.2f)), sector_length, Find_by_endpoint(endpoint2).id);
		// 	qa.Enqueue(connection_point);
		// 	lower_by -=1;
		// }


		//połącz żyły z arteriami

		while((qv.Count >= 1) && (qa.Count >= 1)){
			endpoint1 = qa.Dequeue();
			endpoint2 = qv.Dequeue();
			CreateLongCylinder( endpoint1, endpoint2, min_thickness/2.0f, sector_length, Find_by_endpoint(endpoint1).id);
		}

	}

	private Tube Find_by_endpoint(Vector3 point){
		foreach(Tube t in export_dict){
			if (t.end == point) return t;
		}
		return null;
	}
	

	private void CreateCylinderBetweenPoints(Vector3 start, Vector3 end, float r)
	{
		float width = r * 2.0f;
		Vector3 offset = end - start;
		Vector3 scale = new Vector3 (width, offset.magnitude / 2.0f, width);
		Vector3 position = start + (offset / 2.0f);
		GameObject cylinder = Instantiate (cylinderPrefab, position, Quaternion.identity) as GameObject;
		cylinder.GetComponent<Renderer> ().material.color = new Color(1,0,0);
		cylinder.transform.parent = vessel.transform;
		cylinder.transform.up = offset;
		cylinder.transform.localScale = scale;
	}

	private Tube CreateLongCylinderWithReturn(Vector3 start, Vector3 end, float width, float sector_length, int parent_id)
	{
		Tube tube;
		float no_of_sectors = Mathf.Floor(Mathf.Sqrt(Mathf.Pow(start.x - end.x,2)+Mathf.Pow(start.y-end.y,2)+Mathf.Pow(start.z-end.z,2))/sector_length);
		Vector3 sector_vec = new Vector3(-(start.x - end.x)/no_of_sectors, -(start.y - end.y)/no_of_sectors, -(start.z - end.z)/no_of_sectors);
		Vector3 startpoint = start;
		Vector3 endpoint;

		for(int i=1; i<=no_of_sectors; i++){
			if (i==1){
				endpoint = new Vector3(start.x + i*sector_vec.x + Random.Range(-max_curvature, max_curvature), start.y + i*sector_vec.y + Random.Range(0.05f, max_curvature), start.z + i*sector_vec.z + Random.Range(-0.2f, 0.2f));
			} else{
				endpoint = new Vector3(start.x + i*sector_vec.x + Random.Range(-max_curvature, max_curvature), start.y + i*sector_vec.y + Random.Range(-max_curvature, max_curvature), start.z + i*sector_vec.z + Random.Range(-0.2f, 0.2f));
			}
			CreateCylinderBetweenPoints(startpoint, endpoint, width);
			export_dict.Add(new Tube(id, startpoint, endpoint, width/2.0f, parent_id));
			parent_id = id;
			id +=1;
			startpoint=endpoint;

		}

		CreateCylinderBetweenPoints(startpoint, end, width);
		tube = new Tube(id, startpoint, end, width/2.0f, parent_id);
		export_dict.Add(tube);
		id +=1;
		return tube;
	}


	private void CreateLongCylinder(Vector3 start, Vector3 end, float width, float sector_length, int parent_id)
	{
		float no_of_sectors = Mathf.Floor(Mathf.Sqrt(Mathf.Pow(start.x - end.x,2)+Mathf.Pow(start.y-end.y,2)+Mathf.Pow(start.z-end.z,2))/sector_length);
		Vector3 sector_vec = new Vector3(-(start.x - end.x)/no_of_sectors, -(start.y - end.y)/no_of_sectors, -(start.z - end.z)/no_of_sectors);
		Vector3 startpoint = start;
		Vector3 endpoint;

		for(int i=1; i<=no_of_sectors; i++){
			if (i==1){
				endpoint = new Vector3(start.x + i*sector_vec.x + Random.Range(-max_curvature, max_curvature), start.y + i*sector_vec.y + Random.Range(0.05f, max_curvature), start.z + i*sector_vec.z + Random.Range(-0.2f, 0.2f));
			} else{
				endpoint = new Vector3(start.x + i*sector_vec.x + Random.Range(-max_curvature, max_curvature), start.y + i*sector_vec.y + Random.Range(-max_curvature, max_curvature), start.z + i*sector_vec.z + Random.Range(-0.2f, 0.2f));
			}
			CreateCylinderBetweenPoints(startpoint, endpoint, width);
			export_dict.Add(new Tube(id, startpoint, endpoint, width/2.0f, parent_id));
			parent_id = id;
			id +=1;
			startpoint=endpoint;

		}
		Tube tube;
		CreateCylinderBetweenPoints(startpoint, end, width);
		tube = new Tube(id, startpoint, end, width/2.0f, parent_id);
		export_dict.Add(tube);
		id +=1;
	}

	private void CreateLongCylinderWithMove(Vector3 start, Tube tube, float width, float sector_length, int parent_id, int top_id)
	{
		Vector3 end = new Vector3((tube.start.x + tube.end.x)/2.0f, (tube.start.y + tube.end.y)/2.0f, (tube.start.z + tube.end.z)/2.0f);
		float no_of_sectors = Mathf.Floor(Mathf.Sqrt(Mathf.Pow(start.x - end.x,2)+Mathf.Pow(start.y-end.y,2)+Mathf.Pow(start.z-end.z,2))/sector_length);
		Vector3 sector_vec = new Vector3(-(start.x - end.x)/no_of_sectors, -(start.y - end.y)/no_of_sectors, -(start.z - end.z)/no_of_sectors);
		Vector3 startpoint = start;
		Vector3 endpoint;

		for(int i=1; i<=no_of_sectors; i++){
			if (i==1){
				endpoint = new Vector3(start.x + i*sector_vec.x + Random.Range(-max_curvature, max_curvature), start.y + i*sector_vec.y + Random.Range(0.05f, max_curvature), start.z + i*sector_vec.z + Random.Range(-0.2f, 0.2f));
			} else{
				endpoint = new Vector3(start.x + i*sector_vec.x + Random.Range(-max_curvature, max_curvature), start.y + i*sector_vec.y + Random.Range(-max_curvature, max_curvature), start.z + i*sector_vec.z + Random.Range(-0.2f, 0.2f));
			}
			CreateCylinderBetweenPoints(startpoint, endpoint, width);
			export_dict.Add(new Tube(id, startpoint, endpoint, width/2.0f, parent_id));
			parent_id = id;
			id +=1;
			startpoint=endpoint;

		}
		CreateCylinderBetweenPoints(startpoint, end, width);
		export_dict.Add(new Tube(id, startpoint, end, width/2.0f, parent_id, -1, top_id));
		id +=1;
	}

	private List<Tube> CreateLongCylinderWithReturnPoints(Vector3 start, Vector3 end, float width, float sector_length)
	{
		float no_of_sectors = Mathf.Floor(Mathf.Sqrt(Mathf.Pow(start.x - end.x,2)+Mathf.Pow(start.y-end.y,2)+Mathf.Pow(start.z-end.z,2))/sector_length);
		Vector3 sector_vec = new Vector3(-(start.x - end.x)/no_of_sectors, -(start.y - end.y)/no_of_sectors, -(start.z - end.z)/no_of_sectors);
		Vector3 startpoint = start;
		Vector3 endpoint;
		List<Tube> q = new List<Tube>();
		Tube tube;

		for(int i=1; i<=no_of_sectors; i++){
			endpoint = new Vector3(start.x + i*sector_vec.x + Random.Range(-max_curvature,max_curvature), start.y + i*sector_vec.y + Random.Range(-max_curvature, max_curvature), start.z + i*sector_vec.z + Random.Range(-max_curvature, max_curvature));
			CreateCylinderBetweenPoints(startpoint, endpoint, width);
			if(i==1){
				tube = new Tube(id,startpoint,endpoint,width/2.0f,-1,-1,-1,true,true,-10.0f);
				id+=1;
			}else{
				tube = new Tube(id,startpoint,endpoint,width/2.0f, id-1);
				id+=1;
			}

			q.Add(tube);
			startpoint=endpoint;

		}

		CreateCylinderBetweenPoints(startpoint, end, width);
		tube = new Tube(id,startpoint,end,width/2.0f,id-1,-1,-1,false,true,10.0f);
		id+=1;
		q.Add(tube);
		return q;
	}

	// Update is called once per frame
	void Update () {
		if(params_changed){
			Generate();
		}
	
	}
}