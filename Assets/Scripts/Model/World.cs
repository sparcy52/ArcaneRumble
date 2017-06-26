using System;
using System.IO;
using System.Net;
using UnityEngine;
using System.Collections.Generic;

namespace Model
{
	public class World
	{
		public readonly int W, E;
		private Hex[,] _terrain;
        public TextAsset[] maplist;
        /*public World(int w, int e)
		{
			// TODO: implement properly
			W = w;
			E = e;
			_terrain = new Hex[w, e];
			for (var iw = 0; iw < w; iw++)
			{
				for (var ie = 0; ie < e; ie++)
				{
					_terrain[iw, ie] = new Hex((HexType) ((iw+ie)%3));
				}
			}
		}*/
        public World(TextAsset[] maplist,  Player[] Players)
        {
			List<TileVector> player1hexpos = new List<TileVector>();
			List<TileVector> player2hexpos = new List<TileVector>();
            this.maplist = maplist;
            StringReader reader = null;
            System.Random rnd = new System.Random();
            int map = rnd.Next(0, maplist.Length);

            TextAsset mapData = maplist[map];

            reader = new StringReader(mapData.text);
            string line;
            line = reader.ReadLine();
            String[] mapsize = line.Split('\t');
            W = int.Parse(mapsize[0]);
            E = int.Parse(mapsize[1]);
            int i = 0;
            _terrain = new Hex[W, E];
            while ((line = reader.ReadLine()) != null){
                //just hextype
                string[] mapline = line.Split('\t');
                for (int j = 0; j < mapline.Length; j++)
                {
                    if (mapline[j] == "-")
                    {
                        //Do nothing
                    }
                    else if (mapline[j] == "w")
                    {
                        _terrain[i, j] = new Hex(HexType.Water);
                    }
                    else if (mapline[j] == "*")
                    {
                        _terrain[i, j] = new Hex(HexType.Objective, 1);
                    }
                    else if (mapline[j] == "p1")
                    {
                        _terrain[i, j] = new Hex(HexType.Deploy, Players[0]);
						player1hexpos.Add(new TileVector (i,j));
                    }
                    else if (mapline[j] == "p2")
                    {
                        _terrain[i, j] = new Hex(HexType.Deploy, Players[1]);
						player2hexpos.Add(new TileVector (i,j));
                    }
                    else
                    {
                        HexType type = (HexType)int.Parse(mapline[j]);
                        _terrain[i, j] = new Hex(type);
                    }

                }
                i++;
            }
            reader.Close();
			Players [0].PlayerPlacables = player1hexpos;
			Players [1].PlayerPlacables = player2hexpos;
        }
        /*public World(int map, int mapsize)
        {
            W = mapsize;
            E = mapsize;
            _terrain = new Hex[mapsize, mapsize];
            String filename = "map" + map + ".txt";
            StreamReader reader = File.OpenText(filename);
            string line;
            int i = 0;
            while ((line = reader.ReadLine()) != null)
            {
                //just hextype
                string[] mapline = line.Split('\t');
                for(int j = 0; j < mapline.Length; j++)
                {
                    if(mapline[j] == "-")
                    {
                            //Do nothing
                    }
                    else if(mapline[j] == "o")
                    {
                        _terrain[i, j] = new Hex(HexType.Dirt, 1);
                    }
                    else
                    {
                        HexType type = (HexType)int.Parse(mapline[j]);
                        _terrain[i, j] = new Hex(type);
                    }
                }
                i++;
            }
        }*/


        public Hex this[TileVector pos]
		{
			get { return this[pos.W, pos.E]; }
		}

		public Hex this[int w, int e]
		{
			get
			{
				if (IsInRange(w, e)) return _terrain[w, e];
				else return null;
			}
		}

		private bool IsInRange(int w, int e)
		{
			return w >= 0 && w < W &&
			       e >= 0 && e < E;
		}
	}

    public class Hex
    {
        /// <summary>
        /// The Unit that is currently in this Hex, null if there is none present.
        /// </summary>
        public Unit Occupant { get; set; }  // TODO: revise this design decision

        /// <summary>
        /// The type of terrain this represents.
        /// </summary>
        public HexType Type { get; private set; } 	// replace with a delegate to implement more complex hex behavior

        public Objective Objective { get; set; }
        public bool Impassable { get; set; }
        public bool Placeable { get; set; }
        public Player Owner { get; set; }


        // public int Height { get; private set; }; // for if we want to implement height later

        public Hex(HexType type)
        {
            Type = type;
            Occupant = null;
            Objective = null;
            if (type == HexType.Water)
            {
                Impassable = true;
            }
            else
            {
                Impassable = false;
            }
            Placeable = false;
            Owner = null;
        }

        public Hex(HexType type, int i)
        {
            Type = type;
            Occupant = null;
            Objective = new Objective();
            Impassable = false;
            Placeable = false;
            Owner = null;
        }

        public Hex(HexType type,  Player player)
        {
            Type = type;
            Occupant = null;
            Objective = null;
            Impassable = false;
            Placeable = true;
            Owner = player;
        }
        public bool HasObjective()
        {
            if (Objective == null)
            {
                return false;
            }
            else {
                return true;
            }
        }
    }
    public class Objective
    {
        public Player controllingPlayer { get; set; }

        public Objective()
            {
                controllingPlayer = null;
            }
    }
    
    


	public enum HexType
	{
		Dirt,
		Stone,
		Snow,
        Water,
        Objective,
        Deploy
	}
}