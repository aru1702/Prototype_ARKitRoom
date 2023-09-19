using UnityEngine;

public class MyObject
{
    public string name { get; set; }
    public string coordinate_system { get; set; }
    public float length { get; set; }
    public float height { get; set; }
    public float width { get; set; }
    public Origin origin { get; set; }
    public VirtualObject virtualObject { get; set; }
    public bool iotDevice_true { get; set; }
    public string comment { get; set; }

    public class Origin
    {
        public string type { get; set; }
        public string descriptor { get; set; }
        public Origin(string type, string descriptor) {
            this.type = type;
            this.descriptor = descriptor;
        }
    }

    public class VirtualObject
    {
        public string type { get; set; }
        public Special special { get; set; }

        public VirtualObject(string type, Special special)
        {
            this.type = type;
            this.special = special;
        }

        public VirtualObject(string type, string special_parameter, Vector3 special_position)
        {
            this.type = type;
            special = new(special_parameter, special_position);
        }

        public VirtualObject(string type, string special_parameter, string pos_x, string pos_y, string pos_z)
        {
            Vector3 special_position = new(float.Parse(pos_x), float.Parse(pos_y), float.Parse(pos_z));
            this.type = type;
            special = new(special_parameter, special_position);
        }

        [Tooltip("Special position in string with delimiter")]
        public VirtualObject(string type, string special_parameter, string special_position, string delimiter = ";")
        {
            if (type == PrefabType.SPECIAL)
            {
                string[] strSplit = special_position.Split(delimiter);
                Vector3 position = new(float.Parse(strSplit[0]), float.Parse(strSplit[1]), float.Parse(strSplit[2]));
                this.type = type;
                special = new(special_parameter, position);
            }
            else
            {
                this.type = type;
                special = new(special_parameter, new(0, 0, 0));
            }

        }

        public class Special
        {
            public string parameter { get; set; }
            public Vector3 position { get; set; }

            public Special(string parameter, Vector3 position)
            {
                this.parameter = parameter;
                this.position = position;
            }

            public Special(string parameter, string pos_x, string pos_y, string pos_z)
            {
                Vector3 position = new(float.Parse(pos_x), float.Parse(pos_y), float.Parse(pos_z));
                this.parameter = parameter;
                this.position = position;
            }
        }
    }

    public struct OriginType
    {
        public static string EDGE = "EDGE";
        public static string PLANECENTER = "PLANECENTER";
        public static string DIMENSIONCENTER = "DIMENSIONTCENTER";
    }

    public struct OriginDescriptor
    {
        public static string LUF = "LUF";
        public static string LUB = "LUB";
        public static string LDF = "LDF";
        public static string LDB = "LDB";
        public static string RUF = "RUF";
        public static string RUB = "RUB";
        public static string RDF = "RDF";
        public static string RDB = "RDB";

        public static string L = "L";
        public static string R = "R";
        public static string U = "U";
        public static string D = "D";
        public static string F = "F";
        public static string B = "B";
    }

    public struct PrefabType
    {
        public static string CUBE = "CUBE";
        public static string SPHERE = "SPHERE";
        public static string CYLINDER = "CYLINDER";
        public static string SPECIAL = "SPECIAL";
    }

    public struct MyObject_LHW
    {
        public float L { get; set; }
        public float H { get; set; }
        public float W { get; set; }
        public MyObject_LHW(float l, float h, float w) { L = l; H = h; W = w; }

        public MyObject_LHW Zero() { return new MyObject_LHW(0, 0, 0); }
    }

    /**
     * <summary>Default constructor</summary>
     */
    public MyObject(
        string name,
        string coordinate_system,
        float length,
        float height,
        float width,
        Origin origin,
        VirtualObject virtualObject,
        bool iotDevice_true,
        string comment)
    {
        this.name = name;
        this.coordinate_system = coordinate_system;
        this.length = length;
        this.height = height;
        this.width = width;
        this.origin = origin;
        this.virtualObject = virtualObject;
        this.iotDevice_true = iotDevice_true;
        this.comment = comment;
    }

    /**
     * <summary>Constructor for default csv file</summary>
     */
    public MyObject(
        string name,
        string coordinate_system,
        float length,
        float height,
        float width,
        string origin_type,
        string origin_descriptor,
        string virtualobject_type,
        string virtualobject_special_parameter,
        string virtualobject_special_position,
        bool iotDevice_true = true,
        string comment = "")
    {
        this.name = name;
        this.coordinate_system = coordinate_system;
        this.length = length;
        this.height = height;
        this.width = width;
        origin = new(origin_type, origin_descriptor);
        virtualObject = new(virtualobject_type, virtualobject_special_parameter, virtualobject_special_position);
        this.iotDevice_true = iotDevice_true;
        this.comment = comment;
    }

    /**
     * <summary>Constructor for no origin, no virtualObject, and no iot information</summary>
     */
    public MyObject(string name, string coordinate_system, float length, float height, float width)
    {
        this.name = name;
        this.coordinate_system = coordinate_system;
        this.length = length;
        this.height = height;
        this.width = width;
        origin = new(OriginType.DIMENSIONCENTER, "C");
        virtualObject = new(PrefabType.CUBE, null);
        iotDevice_true = true;
        comment = "";
    }

    public override string ToString()
    {
        string returnStr = "Name: " + name + "\n" +
                            "Origin used: " + coordinate_system + "\n" +
                            "Dimension (L,H,W): " + length + "," + height + "," + width + "," + "\n" +
                            "Origin: " + origin.type + " with " + origin.descriptor + "\n" +
                            "Virtual object: " + virtualObject.type + " which if special: \n" +
                                "-> " + virtualObject.special.parameter + "\n" +
                                "-> " + virtualObject.special.position.ToString() + "\n" +
                            "An IoT device? " + iotDevice_true + "\n" +
                            "Comment: " + comment;
        return returnStr;
    }
}
