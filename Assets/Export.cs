using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Export : MonoBehaviour {
	public void ExportToMotpuca(){
		GameObject vessel = GameObject.Find("GameObject");
        generate script = vessel.GetComponent<generate>();

	    //create Folder
	    if (!Directory.Exists ("./Data/")) {
	 
	         Directory.CreateDirectory ("./Data/");
		}
		var file = File.CreateText("skin.ag");

		Rename_ids(script.export_dict);

        foreach (Tube tube in script.export_dict)
		{	
			file.WriteLine("tube");
			file.WriteLine("{");
			file.WriteLine();
			file.WriteLine("m_id = {0}", tube.m_id);
			file.WriteLine();
			file.WriteLine("pos1 = <{0}, {1}, {2}>", tube.start.x, tube.start.y, tube.start.z);
			file.WriteLine("pos2 = <{0}, {1}, {2}>", tube.end.x, tube.end.y, tube.end.z);
			file.WriteLine("r = {0}", tube.r);
			file.WriteLine();
			file.WriteLine("state = ALIVE");
			file.WriteLine("age = 0");
			file.WriteLine("state_age = 0");
			file.WriteLine();

			if (tube.first){
				file.WriteLine("first");
			}

			if (tube.base_id!=0){
				file.WriteLine("base_id = {0}", tube.base_id);
			}
			if (tube.top_id!=0){
				file.WriteLine("top_id = {0}", tube.top_id);
			}
			if (tube.fixed_blood_pressure){
				file.WriteLine("fixed_blood_pressure = 1");
			}
			if (tube.pressure!=0.0f){
				file.WriteLine("blood_pressure = {0}", tube.pressure);
			}
			file.WriteLine();

			//just for debugging
			// if (tube.prev_id>=0){
			// 	file.WriteLine("prev_id = {0}", tube.prev_id);
			// }
			// file.WriteLine();

			// file.WriteLine("id = {0}", tube.id);
			// file.WriteLine();



			file.WriteLine("}");
		}
	}

	private void Rename_ids(List<Tube> export_dict){
		//find all firsts 
		List<int> firsts = new List<int>();
		int curr_m_id = 1;
		int parent_id;
		int x;

		foreach(Tube tube in export_dict){
			if (tube.first){
				firsts.Add(tube.id);
			}
		}

		//for every first find all the children an id 
		foreach(int id in firsts){
			parent_id = id;
			export_dict[id].set_id(curr_m_id); //set id for first in vessel
			curr_m_id+=1;
			x = Find_by_parent_id(parent_id, export_dict); //x - id of a child

			//find and mark all of the children
			while(x>=0){
				export_dict[x].set_id(curr_m_id);
				parent_id = x;
				curr_m_id+=1;
				x = Find_by_parent_id(parent_id, export_dict);
			}
		}
	}

	private int Find_by_parent_id(int id, List<Tube> export_dict){
		foreach(Tube t in export_dict){
			if (t.prev_id == id) return t.id;
		}
		return -1;
	}
}