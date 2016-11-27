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

		file.WriteLine("tube");
		file.WriteLine("{");
		file.WriteLine();


        foreach (Tube tube in script.export_dict)
		{
			file.WriteLine("id = {0}", tube.id);
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

			file.WriteLine("}");
		}
	}

}