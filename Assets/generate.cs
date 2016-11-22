using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class generate : MonoBehaviour {

	public GameObject cylinderPrefab;
	public GameObject vessel;

	//these variables should be set up in the menu
	public float pairs_of_vessels = 2.0f;
	public float ups_per_vessel = 3.0f;
	public float sector_length = 1.0f;
	public float vessel_length = 30.0f;
	public float artery_vein_dist = 3.0f;
	public float pairs_dist = 5.0f;
	public float max_thickness = 0.5f;
	public float levels = 20.0f;
	public float chance_of_split = 0.05f;
	public float max_distance_of_split = 1.5f; 
	public float max_curvature = 0.3f;

	private bool params_changed = false;



	public System.Collections.Generic.List<Tube> export_dict;

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

	// Use this for initialization
	void Generate () {
		params_changed = false;
		GameObject.Destroy(vessel);
		vessel = new GameObject("Vessel");

		//computed values
		float grid_sectors_per_vessel = vessel_length / sector_length;
		System.Collections.Generic.Queue<Vector3> qv = new Queue<Vector3>();
		System.Collections.Generic.Queue<Vector3> qa = new Queue<Vector3>();
		Vector3 split_point;
		float noise;
		float q_size;
		Vector3 point;
		Vector3 endpoint1;
		Vector3 endpoint2;
		Vector3 connection_point;
		System.Collections.Generic.Queue<Vector3> base_qv = new Queue<Vector3>();
		System.Collections.Generic.Queue<Vector3> base_qa = new Queue<Vector3>();
		export_dict = new List<Tube>();

		//create base vessels
		for(float i=1.0f; i<= pairs_of_vessels; i++){
			base_qv = CreateLongCylinderWithReturnPoints(new Vector3(0,0, i*(pairs_dist+artery_vein_dist)), new Vector3(vessel_length, 0, i*(pairs_dist+artery_vein_dist)), max_thickness, sector_length);
			base_qa = CreateLongCylinderWithReturnPoints(new Vector3(0,0, i*(pairs_dist+artery_vein_dist)+artery_vein_dist), new Vector3(vessel_length,0, i*(pairs_dist+artery_vein_dist)+artery_vein_dist), max_thickness, sector_length);

			for(float k=1.0f; k<= ups_per_vessel; k++){
				noise = Random.Range(0, (int)(base_qv.Count/ups_per_vessel));
				split_point = base_qv.ElementAt( (int) Mathf.Floor((k*base_qv.Count/ups_per_vessel)-1));
				CreateCylinderBetweenPoints(split_point, new Vector3(split_point.x,split_point.y + sector_length, split_point.z), max_thickness);
				qv.Enqueue(split_point);

				noise = Random.Range(0, (int)(base_qa.Count/ups_per_vessel));
				split_point = base_qa.ElementAt( (int)Mathf.Floor((k*base_qa.Count/ups_per_vessel)-1));
				CreateCylinderBetweenPoints(split_point, new Vector3(split_point.x,split_point.y + sector_length, split_point.z), max_thickness);
				qa.Enqueue(split_point);
			}		
		}


		//create veins going up
		for(float lvl = 1.0f; lvl<levels; lvl++){
			q_size = qv.Count;
			for(float v=1.0f; v<=q_size; v++){
				point = qv.Dequeue();
				noise = Random.Range(0.0f, 1.0f);
				if(noise < chance_of_split){
					endpoint1 = new Vector3(point.x + Random.Range(-max_distance_of_split,max_distance_of_split), point.y + sector_length, point.z + Random.Range(-1.0f *max_distance_of_split,max_distance_of_split));
					endpoint2 = new Vector3(point.x + Random.Range(-max_distance_of_split,max_distance_of_split), point.y + sector_length, point.z + Random.Range(-1.0f * max_distance_of_split,max_distance_of_split));
					qv.Enqueue(endpoint1);
					qv.Enqueue(endpoint2);
					CreateCylinderBetweenPoints(point, endpoint1, max_thickness/(1 + Mathf.Floor(lvl*0.2f)));
					CreateCylinderBetweenPoints(point, endpoint2, max_thickness/(1 + Mathf.Floor(lvl*0.2f)));
				} else {
					endpoint1 = new Vector3(point.x + Random.Range(-max_curvature,max_curvature), point.y + sector_length, point.z + Random.Range(-1.0f * max_curvature,max_curvature));
					qv.Enqueue(endpoint1);
					CreateCylinderBetweenPoints(point, endpoint1, max_thickness/(1 + Mathf.Floor(lvl*0.2f)));
				}
			}
		}

		//create arteries going up
		for(float lvl = 1.0f; lvl<levels; lvl++){
			q_size = qa.Count;
			for(float v=1.0f; v<=q_size; v++){
				point = qa.Dequeue();
				noise = Random.Range(0.0f, 1.0f);
				if(noise < chance_of_split){
					endpoint1 = new Vector3(point.x + Random.Range(-max_distance_of_split,max_distance_of_split), point.y + sector_length, point.z + Random.Range(-1.0f *max_distance_of_split,max_distance_of_split));
					endpoint2 = new Vector3(point.x + Random.Range(-max_distance_of_split,max_distance_of_split), point.y + sector_length, point.z + Random.Range(-1.0f * max_distance_of_split,max_distance_of_split));
					qa.Enqueue(endpoint1);
					qa.Enqueue(endpoint2);
					CreateCylinderBetweenPoints(point, endpoint1, max_thickness/(1 + Mathf.Floor(lvl*0.2f)));
					CreateCylinderBetweenPoints(point, endpoint2, max_thickness/(1 + Mathf.Floor(lvl*0.2f)));
				} else {
					endpoint1 = new Vector3(point.x + Random.Range(-max_curvature,max_curvature), point.y + sector_length, point.z + Random.Range(-1.0f * max_curvature,max_curvature));
					qa.Enqueue(endpoint1);
					CreateCylinderBetweenPoints(point, endpoint1, max_thickness/(1 + Mathf.Floor(lvl*0.2f)));
				}
			}
		}


		//zniweluj różnicę pomiędzy ilością zakończeń

		int diff = qv.Count - qa.Count;

		if(diff >0) {
			while(diff >0){
				endpoint1 = qv.Dequeue();
				endpoint2 = qv.Dequeue();
				connection_point = new Vector3((endpoint1.x + endpoint2.x)/2.0f, (endpoint1.y + endpoint2.y)/2.0f, (endpoint1.z + endpoint2.z)/2.0f);
				CreateLongCylinder(endpoint1, connection_point, max_thickness/(1 + Mathf.Floor(levels*0.2f)), sector_length);
				CreateLongCylinder(endpoint2, connection_point, max_thickness/(1 + Mathf.Floor(levels*0.2f)), sector_length);
				qv.Enqueue(connection_point);
				diff -=1;
			}
		} else {
			while(diff <0){
				endpoint1 = qa.Dequeue();
				endpoint2 = qa.Dequeue();
				connection_point = new Vector3((endpoint1.x + endpoint2.x)/2.0f, (endpoint1.y + endpoint2.y)/2.0f, (endpoint1.z + endpoint2.z)/2.0f);
				CreateLongCylinder(endpoint1, connection_point, max_thickness/(1 + Mathf.Floor(levels*0.2f)), sector_length);
				CreateLongCylinder(endpoint2, connection_point, max_thickness/(1 + Mathf.Floor(levels*0.2f)), sector_length);
				qa.Enqueue(connection_point);
				diff +=1;
			}
		}


		qa = new Queue<Vector3>(qa.OrderBy( vec => vec.z+ vec.x*3.0f));
		qv = new Queue<Vector3>(qv.OrderBy( vec => vec.z+ vec.x*3.0f));


		//lower number of connections
		float lower_by = Mathf.Floor(qv.Count/2.0f);

		while(lower_by>0){
			endpoint1 = qv.Dequeue();
			endpoint2 = qv.Dequeue();
			connection_point = new Vector3((endpoint1.x + endpoint2.x)/2.0f, (endpoint1.y + endpoint2.y)/2.0f, (endpoint1.z + endpoint2.z)/2.0f);
			CreateLongCylinder(endpoint1, connection_point, max_thickness/(1 + Mathf.Floor(levels*0.2f)), sector_length);
			CreateLongCylinder(endpoint2, connection_point, max_thickness/(1 + Mathf.Floor(levels*0.2f)), sector_length);
			qv.Enqueue(connection_point);
			endpoint1 = qa.Dequeue();
			endpoint2 = qa.Dequeue();
			connection_point = new Vector3((endpoint1.x + endpoint2.x)/2.0f, (endpoint1.y + endpoint2.y)/2.0f, (endpoint1.z + endpoint2.z)/2.0f);
			CreateLongCylinder(endpoint1, connection_point, max_thickness/(1 + Mathf.Floor(levels*0.2f)), sector_length);
			CreateLongCylinder(endpoint2, connection_point, max_thickness/(1 + Mathf.Floor(levels*0.2f)), sector_length);
			qa.Enqueue(connection_point);
			lower_by -=1;
		}


		//połącz żyły z arteriami

		while((qv.Count >= 1) && (qa.Count >= 1)){
			endpoint1 = qa.Dequeue();
			endpoint2 = qv.Dequeue();
			CreateLongCylinder( endpoint1, endpoint2, max_thickness/(1 + Mathf.Floor(levels*0.2f)), sector_length);
		}

	}
	

	private void CreateCylinderBetweenPoints(Vector3 start, Vector3 end, float width, string type = "")
	{
		Vector3 offset = end - start;
		Vector3 scale = new Vector3 (width, offset.magnitude / 2.0f, width);
		Vector3 position = start + (offset / 2.0f);
		GameObject cylinder = Instantiate (cylinderPrefab, position, Quaternion.identity) as GameObject;
		cylinder.transform.parent = vessel.transform;
		cylinder.transform.up = offset;
		cylinder.transform.localScale = scale;
		Tube tube = new Tube(start,end,width/2.0f);
		export_dict.Add(tube);
	}

	private void CreateLongCylinder(Vector3 start, Vector3 end, float width, float sector_length)
	{
		float no_of_sectors = Mathf.Floor(Mathf.Sqrt(Mathf.Pow(start.x - end.x,2)+Mathf.Pow(start.y-end.y,2)+Mathf.Pow(start.z-end.z,2))/sector_length);
		Vector3 sector_vec = new Vector3(-(start.x - end.x)/no_of_sectors, -(start.y - end.y)/no_of_sectors, -(start.z - end.z)/no_of_sectors);
		Vector3 startpoint = start;
		Vector3 endpoint;

		for(int i=1; i<=no_of_sectors; i++){
			endpoint = new Vector3(start.x + i*sector_vec.x + Random.Range(-max_curvature, max_curvature), start.y + i*sector_vec.y + Random.Range(-max_curvature, max_curvature), start.z + i*sector_vec.z + Random.Range(-0.2f, 0.2f));
			CreateCylinderBetweenPoints(startpoint, endpoint, width);
			startpoint=endpoint;

		}

		CreateCylinderBetweenPoints(startpoint, end, width);
	}

	private Queue<Vector3> CreateLongCylinderWithReturnPoints(Vector3 start, Vector3 end, float width, float sector_length)
	{
		float no_of_sectors = Mathf.Floor(Mathf.Sqrt(Mathf.Pow(start.x - end.x,2)+Mathf.Pow(start.y-end.y,2)+Mathf.Pow(start.z-end.z,2))/sector_length);
		Vector3 sector_vec = new Vector3(-(start.x - end.x)/no_of_sectors, -(start.y - end.y)/no_of_sectors, -(start.z - end.z)/no_of_sectors);
		Vector3 startpoint = start;
		Vector3 endpoint;
		Queue<Vector3> q = new Queue<Vector3>();

		for(int i=1; i<=no_of_sectors; i++){
			endpoint = new Vector3(start.x + i*sector_vec.x + Random.Range(-max_curvature,max_curvature), start.y + i*sector_vec.y + Random.Range(-max_curvature, max_curvature), start.z + i*sector_vec.z + Random.Range(-max_curvature, max_curvature));
			CreateCylinderBetweenPoints(startpoint, endpoint, width);
			q.Enqueue(endpoint);
			startpoint=endpoint;

		}

		CreateCylinderBetweenPoints(startpoint, end, width);
		return q;
	}

	// Update is called once per frame
	void Update () {
		if(params_changed){
			Generate();
		}
	
	}
}