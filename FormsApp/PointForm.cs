using Newtonsoft.Json;
//using PointLib;
using PointLibFr;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Windows.Forms;
using System.Xml.Serialization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FormsApp
{
    public partial class PointForm : Form
    {
        private Point[] points = null;

        public PointForm()
        {
            InitializeComponent();
        }

        private void brnCreate_Click(object sender, System.EventArgs e)
        {
            points = new Point[5];

            var rnd = new Random();

            for (int i = 0; i < points.Length; i++)
                points[i] = rnd.Next(3) % 2 == 0 ? new Point() : new Point3D();

            listBox.DataSource = points;
        }

        private void btnSort_Click(object sender, System.EventArgs e)
        {
            if (points == null)
                return;

            Array.Sort(points);

            listBox.DataSource = null;
            listBox.DataSource = points;
        }

        private void btnSerialize_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "SOAP|*.soap|XML|*.xml|JSON|*.json|Binary|*.bin|YAML|*.yaml|My|*.my";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            using (var fs =
                new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write))
            {
                switch (Path.GetExtension(dlg.FileName))
                {
                    case ".bin":
                        var bf = new BinaryFormatter();
                        bf.Serialize(fs, points);
                        break;
                    case ".soap":
                        var sf = new SoapFormatter();
                        sf.Serialize(fs, points);
                        break;
                    case ".xml":
                        var xf = new XmlSerializer(typeof(Point[]), new[] { typeof(Point3D) });
                        xf.Serialize(fs, points);
                        break;
                    case ".json":
                        var options = new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Objects,
                        };

                        var jf = JsonSerializer.Create(options);

                        using (var w = new StreamWriter(fs))
                            jf.Serialize(w, points);
                        break;
                    case ".yaml":
                        var yf = new SerializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .WithTagMapping("!Point", typeof(Point))
                            .WithTagMapping("!Point3D", typeof(Point3D))
                            .Build();
                        using (var w = new StreamWriter(fs))
                            yf.Serialize(w, points);
                        break;
                    case ".my":
                        var mf = new CustomSerializer();
                        using (var w = new StreamWriter(fs))
                            mf.Serialize(w, points);
                        break;
                }
            }
        }

        private void btnDeserialize_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "SOAP|*.soap|XML|*.xml|JSON|*.json|Binary|*.bin|YAML|*.yaml|My|*.my";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            using (var fs =
                new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read))
            {
                switch (Path.GetExtension(dlg.FileName))
                {
                    case ".bin":
                        var bf = new BinaryFormatter();
                        points = (Point[])bf.Deserialize(fs);
                        break;
                    case ".soap":
                        var sf = new SoapFormatter();
                        points = (Point[])sf.Deserialize(fs);
                        break;
                    case ".xml":
                        var xf = new XmlSerializer(typeof(Point[]), new[] { typeof(Point3D) });
                        points = (Point[])xf.Deserialize(fs);
                        break;
                    case ".json":
                        var options = new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Objects,
                        };
                        var jf = JsonSerializer.Create(options);

                        using (var r = new StreamReader(fs))
                            points = (Point[])jf.Deserialize(r, typeof(Point[]));

                        break;
                    case ".yaml":
                        var yf = new DeserializerBuilder()
                            .WithNamingConvention(UnderscoredNamingConvention.Instance)
                            .WithTagMapping("!Point", typeof(Point))
                            .WithTagMapping("!Point3D", typeof(Point3D))
                            .Build();

                        using (var r = new StreamReader(fs))
                            points = (Point[])yf.Deserialize(r, typeof(Point[]));

                        break;
                    case ".my":
                        var mf = new CustomSerializer();
                        using (var r = new StreamReader(fs))
                            points = (Point[])mf.Deserialize(r);
                        break;
                }
            }

            listBox.DataSource = null;
            listBox.DataSource = points;
        }
    }
}
