using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Export : MonoBehaviour {
	public void ExportToMotpuca(){
		GameObject vessel = GameObject.Find("GameObject");
        generate script = vessel.GetComponent<generate>();

        int time_of_death = 3600;

	    //create Folder
	    if (!Directory.Exists ("./Data/")) {
	 
	         Directory.CreateDirectory ("./Data/");
		}
		var file = File.CreateText("skin.ag");

		file.WriteLine("Visual\r\n{\r\n bkg_color = <0.18, 0.24, 0.3, 1>\r\n axis_x_color = <0.75, 0, 0, 1>\r\n axis_y_color = <0, 0.75, 0, 1>\r\n axis_z_color = <0, 0, 0.75, 1>\r\n comp_box_color = <0.5, 0.5, 0.5, 1>\r\n in_barrier_color = <1, 1, 0, 0.5>\r\n out_barrier_color = <1, 0, 0, 0.5>\r\n cell_alive_color = <0, 0, 1, 1>\r\n cell_hypoxia_color = <0, 1, 0, 1>\r\n cell_apoptosis_color = <0.5, 0.5, 0.5, 1>\r\n cell_necrosis_color = <0.2, 0.2, 0.2, 1>\r\n tube_color = <0.65, 0, 0, 1>\r\n clip_plane_color = <1, 1, 1, 1>\r\n navigator_color = <1, 0.64, 0, 0.75>\r\n boxes_color = <1, 1, 1, 0.25>\r\n}");

		file.WriteLine("Simulation\r\n{\r\n dimensions = 3\r\n sim_phases = -1\r\n time_step = 1\r\n time = 0\r\n stop_time = inf");
		file.WriteLine(" comp_box_from = <{0}, {1}, {2}>", (-script.size_x /2.0f) * 100.0f, (-script.size_y /2.0f) * 100.0f, (-script.size_z /2.0f) * 100.0f);
		file.WriteLine(" comp_box_to = <{0}, {1}, {2}>", (script.size_x /2.0f) * 100.0f , (script.size_y /2.0f) * 100.0f, (script.size_z /2.0f) * 100.0f);
		file.WriteLine(" box_size = 100");
		file.WriteLine(" max_cells_per_box = 100\r\n force_r_cut = 10\r\n max_tube_chains = 1000\r\n max_tube_merge = 20\r\n save_statistics = 0\r\n save_povray = 0\r\n save_ag = 0\r\n graph_sampling = 10\r\n diffusion_coeff_O2 = 4000\r\n diffusion_coeff_TAF = 1000\r\n diffusion_coeff_Pericytes = 10\r\n}");

		file.WriteLine();

		file.WriteLine("TubularSystem\r\n{\r\n force_chain_attr_factor = 1e-014\r\n force_length_keep_factor = 1e-014\r\n force_angle_factor = 5e-016\r\n force_rep_factor = 1e-014\r\n force_atr1_factor = 5e-015\r\n force_atr2_factor = 1e-015\r\n density = 1\r\n lengthening_speed = 0.1\r\n thickening_speed = 0.0001\r\n minimum_interphase_time = 1000\r\n TAFtrigger = 0.2\r\n minimum_blood_flow = 0.01\r\n time_to_degradation = inf\r\n o2_production = 3e-022\r\n}");

		file.WriteLine("Barrier\r\n{\r\n type = KEEP_IN");
		file.WriteLine(" from = <{0}, {1}, {2}>", (-script.size_x /2.0f) * 100.0f, (-script.size_y /2.0f) * 100.0f, (-script.size_z /2.0f) * 100.0f);
		file.WriteLine(" to = <{0}, {1}, {2}>", (script.size_x /2.0f) * 100.0f, (script.size_y /2.0f) * 100.0f, (script.size_z /2.0f) * 100.0f);
		file.WriteLine(" trans = <1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1>\r\n}");

		file.WriteLine("Tissue\r\n{\r\n name = \"epidermis\" \r\n type = NORMAL\r\n color = <0.882353, 0.847059, 0.65098, 1>");
		file.WriteLine(" cell_r = {0}", script.cell_size * 100.0f/2.0f);
		file.WriteLine(" density = 1\r\n cell_grow_speed = 0.01\r\n minimum_interphase_time = 600\r\n time_to_apoptosis = {0}", time_of_death);
		file.WriteLine(" time_to_necrosis = 200\r\n time_in_necrosis = 200\r\n dead_r = 8\r\n cell_shrink_speed = 0.1\r\n minimum_mitosis_r = 14\r\n force_rep_factor = 1e-014\r\n force_atr1_factor = 5e-015\r\n force_atr2_factor = 1e-015\r\n max_pressure = 1e-016\r\n o2_consumption = 0\r\n o2_hypoxia = 0\r\n pericyte_production = 0.0001\r\n time_to_necrosis_var = 0\r\n force_dpd_factor = 0\r\n dpd_temperature = 0\r\n}");
		

		file.WriteLine("Tissue\r\n{\r\n name = \"dermis\" \r\n type = NORMAL\r\n color = <1, 0.784314, 0.352941, 1>");
		file.WriteLine(" cell_r = {0}", script.cell_size * 100.0f/2.0f);
		file.WriteLine(" density = 1\r\n cell_grow_speed = 0.01\r\n minimum_interphase_time = 600\r\n time_to_apoptosis = {0}", time_of_death);
		file.WriteLine(" time_to_necrosis = 200\r\n time_in_necrosis = 200\r\n dead_r = 8\r\n cell_shrink_speed = 0.1\r\n minimum_mitosis_r = 14\r\n force_rep_factor = 1e-014\r\n force_atr1_factor = 5e-015\r\n force_atr2_factor = 1e-015\r\n max_pressure = 1e-016\r\n o2_consumption = 5e-011\r\n o2_hypoxia = 0.01\r\n pericyte_production = 0.0001\r\n time_to_necrosis_var = 0\r\n force_dpd_factor = 0\r\n dpd_temperature = 0\r\n}");

		file.WriteLine("Tissue\r\n{\r\n name = \"hypodermis\" \r\n type = NORMAL\r\n color = <1, 0.435294, 0.435294, 1>");
		file.WriteLine(" cell_r = {0}", script.cell_size * 100.0f/2.0f);
		file.WriteLine(" density = 1\r\n cell_grow_speed = 0.01\r\n minimum_interphase_time = 600\r\n time_to_apoptosis = {0}", time_of_death);
		file.WriteLine(" time_to_necrosis = 200\r\n time_in_necrosis = 200\r\n dead_r = 8\r\n cell_shrink_speed = 0.1\r\n minimum_mitosis_r = 14\r\n force_rep_factor = 1e-014\r\n force_atr1_factor = 5e-015\r\n force_atr2_factor = 1e-015\r\n max_pressure = 1e-016\r\n o2_consumption = 1e-010\r\n o2_hypoxia = 0.01\r\n pericyte_production = 0.0001\r\n time_to_necrosis_var = 0\r\n force_dpd_factor = 0\r\n dpd_temperature = 0\r\n}");

		file.WriteLine("Tissue\r\n{\r\n name = \"melanoma\" \r\n type = TUMOR\r\n color = <0.47451, 0.247059, 0.0862745, 1>");
		file.WriteLine(" cell_r = {0}", script.cell_size * 100.0f/2.0f);
		file.WriteLine(" density = 1\r\n cell_grow_speed = 0.01\r\n minimum_interphase_time = 600\r\n time_to_apoptosis= {0}", time_of_death);
		file.WriteLine(" time_to_necrosis = 200\r\n time_in_necrosis = 200\r\n dead_r = 8\r\n cell_shrink_speed = 0.1\r\n minimum_mitosis_r = 14\r\n force_rep_factor = 1e-014\r\n force_atr1_factor = 5e-015\r\n force_atr2_factor = 1e-015\r\n max_pressure = 1e-016\r\n o2_consumption = 1e-010\r\n o2_hypoxia = 0.01\r\n pericyte_production = 0.0001\r\n time_to_necrosis_var = 0\r\n force_dpd_factor = 0\r\n dpd_temperature = 0\r\n}");


		Rename_ids(script.export_dict);


		List<Tube> sortedList = script.export_dict.OrderBy(o=>o.m_id).ToList();

        foreach (Tube tube in sortedList)
		{	
			file.WriteLine("tube");
			file.WriteLine("{");
			file.WriteLine();
			file.WriteLine("id = {0}", tube.m_id);
			file.WriteLine();
			file.WriteLine("pos1 = <{0}, {1}, {2}>", tube.start.x * 100.0f, tube.start.y * 100.0f, tube.start.z * 100.0f);
			file.WriteLine("pos2 = <{0}, {1}, {2}>", tube.end.x * 100.0f, tube.end.y * 100.0f, tube.end.z * 100.0f);
			file.WriteLine("r = {0}", tube.r * 100.0f);
			file.WriteLine();
			file.WriteLine("state = ALIVE");
			file.WriteLine("age = 0");
			file.WriteLine("state_age = 0");
			file.WriteLine();

			if (tube.first){
				file.WriteLine("first");
			}

			if (tube.base_id>=0){
				file.WriteLine("base_id = {0}", tube.base_id);
			}
			if (tube.top_id>=0){
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
		int age;

		foreach(Cell cell in script.export_cells){
			file.WriteLine("Cell\r\n{");
			file.WriteLine("tissue = \"{0}\"", cell.tissue);
 			file.WriteLine("state = ALIVE");
 			file.WriteLine("pos = <{0}, {1}, {2}>", cell.position.x * 100.0f, cell.position.y * 100.0f, cell.position.z * 100.0f);
 			file.WriteLine("conc_O2 = 0.5");
 			age = Random.Range(0, time_of_death);
 			file.WriteLine("age = {0}", age);
  			file.WriteLine("state_age = {0}", age);
  			file.WriteLine("}");
		}


		file.Close();

	}

	private void Rename_ids(List<Tube> export_dict){
		//find all firsts 
		List<int> firsts = new List<int>();
		int curr_m_id = 0;
		int parent_id;
		int prev_id;
		int x;

		foreach(Tube tube in export_dict){
			if (tube.first){
				firsts.Add(tube.id);
			}
		}

		//for every first find all the children an id 
		for(int id=0; id<firsts.Count; id++){
			if(firsts[id]>=0){
				parent_id = firsts[id];
				export_dict[firsts[id]].set_id(curr_m_id); //set id for first in vessel
				curr_m_id+=1;
				x = Find_by_parent_id(parent_id, export_dict); //x - id of a child

				//find and mark all of the children
				while(x>=0){
					export_dict[x].set_id(curr_m_id);
					parent_id = x;
					curr_m_id+=1;
					x = Find_by_parent_id(parent_id, export_dict);
				}

				x = Find_same_vessel(export_dict[parent_id],export_dict); //x here is the id of found connecting vessel
				if(x>=0){
					do{
						export_dict[x].set_id(curr_m_id);
						export_dict[x].switch_ends();
						prev_id = x;
						curr_m_id+=1;
						x = export_dict[x].prev_id;
					}while(!export_dict[x].first); 

					export_dict[x].first = false;
					export_dict[x].set_id(curr_m_id);
					export_dict[x].switch_ends();
					export_dict[x].switch_to_top_id();
					curr_m_id+=1;
					int i = firsts.FindIndex(a => a == x);
					

					firsts[i] = -1;
				}
			}
		}

		foreach(Tube tube in export_dict){
			if (tube.top_id >=0){
				tube.top_id = export_dict[tube.top_id].m_id;
			}
			if (tube.base_id >=0){
				tube.base_id = export_dict[tube.base_id].m_id;
			}
		}


	}

	private int Find_same_vessel(Tube tube, List<Tube> export_dict){
		foreach(Tube t in export_dict){
			if (t.end == tube.end && t.id !=tube.id) return t.id;
		}
		return -1;
	}

	private int Find_by_parent_id(int id, List<Tube> export_dict){
		foreach(Tube t in export_dict){
			if (t.prev_id == id) return t.id;
		}
		return -1;
	}
}