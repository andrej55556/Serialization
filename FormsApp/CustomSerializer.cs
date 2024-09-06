using PointLibFr;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel.Composition;

namespace FormsApp
{
    public class CustomSerializer
    {
        public void Serialize(StreamWriter writer, Point[] points)
        {
            foreach (var point in points)
            {
                if (point is Point3D p3d)
                {
                    writer.WriteLine("~Point3D");
                    writer.WriteLine($"\tz: {p3d.Z}");
                    writer.WriteLine($"x: {p3d.X}\ty: {p3d.Y}");
                }
                else if (point is Point p)
                {
                    writer.WriteLine("~Point");
                    writer.WriteLine($"y: {p.Y}");
                    writer.WriteLine($"\tx: {p.X}");
                }
            }
        }

        public Point[] Deserialize(StreamReader reader)
        {
            var points = new System.Collections.Generic.List<Point>();
            string line = null;
            int x, y, z = 0;

            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split('~');
                if (parts.Length > 1)
                {
                    if (parts[1] == "Point")
                    {
                        line = reader.ReadLine();
                        if (line.StartsWith("y:"))
                        {
                            y = int.Parse(line.Split(':')[1]);

                            line = reader.ReadLine();
                            x = int.Parse(line.Split(':')[1]);
                            points.Add(new Point
                            {
                                X = x,
                                Y = y
                            });
                        }
                    }
                    else if (parts[1] == "Point3D")
                    {
                        line = reader.ReadLine();
                        if (line.StartsWith("\tz:"))
                        {
                            z = int.Parse(line.Split(':')[1]);
                            line = reader.ReadLine();

                            string[] part = line.Split(':');

                            x = int.Parse(line.Split(':')[1][1].ToString());
                            y = int.Parse(line.Split(':')[2]);

                            points.Add(new Point3D
                            {
                                X = x,
                                Y = y,
                                Z = z
                            });
                        }
                    }
                }
            }

            return points.ToArray();
        }
    }
}