using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Export : MonoBehaviour {
	public void ExportToMotpuca(){
		GameObject vessel = GameObject.Find("GameObject");
        generate script = vessel.GetComponent<generate>();

	 //    //create Folder
	 //    if (!Directory.Exists ("./Data/" + usernameGUI)) {
	 
	 //         Directory.CreateDirectory ("./Data/" + usernameGUI);
		// }
		// var sr = File.CreateText(fileName);

        foreach (Tube tube in script.export_dict)
		{
			Debug.Log(System.String.Format("Start = {0} {1} {2}", tube.start.x, tube.start.y, tube.start.z));
			Debug.Log(System.String.Format("End = {0} {1} {2}", tube.end.x, tube.end.y, tube.end.z));
			Debug.Log(System.String.Format("R = {0} ", tube.r));
		}
	}

}