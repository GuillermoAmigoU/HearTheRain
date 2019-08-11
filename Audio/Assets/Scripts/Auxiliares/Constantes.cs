using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Auxiliar
{
    public static class  Constantes
    {
        //¿Can player turn? Iteration 4
        public static bool CAN_TURN_RIGHT = false;
        public static bool CAN_TURN_LEFT = false;

        //Iteration 5
        public enum Orientation
        {
            FORWARD,
            BACKWARDS,
            RIGHT,
            LEFT
        };

        //--------------------DEFINITIVE-------------
        //Actual Orientation
        public static Orientation myOrientation;

        //Map Orientation
        public static string GAME_ORIENTATION;

        //Main Player
        public static GameObject MAIN_PLAYER;

        //GameUI
        public static GameObject MAIN_UI;


        //Are we in a puzzle?
        public static bool IS_IN_PUZZLE;

        //Puzzle solved
        public static bool IS_PUZZLE1_SOLVED = false;
        public static bool IS_PUZZLE2_SOLVED = false;
        public static bool IS_PUZZLE3_SOLVED = false;
        public static bool IS_PUZZLE4_SOLVED = false;
        public static bool IS_PUZZLE5_SOLVED = false;
        public static bool IS_PUZZLE6_SOLVED = false;
        public static bool IS_PUZZLE7_SOLVED = false;

        //Parámetros para suelos y paredes
        //Suelos
        public static string MAT_S_PAVEMENT = "Pavement";
        public static string MAT_S_SHATTEREDGLASS = "ShatteredGlass";
        public static string MAT_S_TILE = "Tile";
        public static string MAT_S_WOOD = "Wood";
        public static string MAT_S_WOOD2 = "Wood2";
        public static string MAT_S_WATER = "Water";
        public static string MAT_S_STAIRS = "Stairs";
        
        //Paredes
        public static string MAT_P_PLASTER = "Plaster";
        public static string MAT_P_PAVEMENT="Pavement";
        public static string MAT_P_TILE = "Tile";
        public static string MAT_P_WOOD = "Wood";
        public static string MAT_P_BRICK = "Brick";
        public static string MAT_P_METAL = "Metal";
        public static string MAT_P_LockedDoor = "LockedDoor";
        public static string MAT_P_MainDoor = "MainDoor";
        public static string MAT_P_MainDoor2 = "MainDoor2";

        //Tutorial Paredes
        public static string WALL_TUTORIAL1 = "Tutorial";
        public static string WALL_TUTORIAL2 = "NotFirstTime";

        //Booleanos 
        //Para habiitar o deshabilitar  el sonido de chasquear dedos
        public static bool DONT_SNAP = false;
        public static bool IS_HITTING_WALL;
        public static bool CAN_MOVE = true;
        public static bool IS_SNAPPING = false;
        public static bool IS_PLAYER_LOADED = false;

        //Puzzle solved
        public static int AUDIOS_PLAYED = 0;
        public static int SAVED_USED = 0;


    }
}
