using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class Export : MonoBehaviour {
	public void ExportToMotpuca(){
		GameObject vessel = GameObject.Find("GameObject");
        generate script = vessel.GetComponent<generate>();
        foreach (KeyValuePair<Vector3, Vector3> kvp in script.export_dict)
		{
		    Debug.Log(System.String.Format("Key = {0} {1} {2}, Value = {3} {4} {5}", kvp.Key.x, kvp.Key.y, kvp.Key.z, kvp.Value.x, kvp.Value.y, kvp.Value.z));
		}
	}

}