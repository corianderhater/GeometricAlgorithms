using System;
using System.Collections.Generic;
using System.Numerics;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;

namespace SuperSuper.Fractals.Quaternions
{
    public class QuaterionFractalComponent : GH_Component
    {

        public QuaterionFractalComponent()
          : base("QuaterionFractal", "QF",
              "Description",
              "SuperSuper", "Fractals")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Pts", "Pts", "", GH_ParamAccess.list);
            pManager.AddIntegerParameter("MaxIterations", "MI", "", GH_ParamAccess.item, 100);
            pManager.AddVectorParameter("Vc", "Vc", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Wc", "Wc", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Wz", "Wz", "", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Value", "V", "", GH_ParamAccess.list);
            pManager.AddGenericParameter("PointCloud", "PC", "", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var totalTimestamp = DateTime.Now.Ticks;

            var pts = new List<Point3d>();
            int maxIterations = 0;
            Vector3d Vc = new Vector3d();
            double Wc = 0;
            double Wz = 0;
            if (!DA.GetDataList(0, pts)) return;
            if (!DA.GetData(1, ref maxIterations)) return;
            if (!DA.GetData(2, ref Vc)) return;
            if (!DA.GetData(3, ref Wc)) return;
            if (!DA.GetData(4, ref Wz)) return;

            var result = new List<int>();
            var pc = new PointCloud();
            foreach(var pt in pts)
            {
                var z = new Vector4((float)pt.X, (float)pt.Y, (float)pt.Z, (float)Wz);
                var c = new Vector4((float)Vc.X, (float)Vc.X, (float)Vc.X, (float)Wc);

                var value = Quaternions.ComputeQuaterionStability(z, c, maxIterations);
                result.Add(value);
                var nVal = (value / maxIterations) * 255;
                pc.Add(pt, System.Drawing.Color.FromArgb(nVal, nVal, 0, 0));
            }

            DA.SetDataList(0, result);
            DA.SetData(1, pc);

            RhinoApp.WriteLine("Quaterion fractal processed in: {0} ms", (DateTime.Now.Ticks - totalTimestamp) / 10000);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("F0C73CDB-7E20-4C3C-9467-DCF45E7343B4"); }
        }
    }
}