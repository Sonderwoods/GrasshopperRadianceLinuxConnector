﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace GrasshopperRadianceLinuxConnector.Components
{
    public class GH_Download : GH_Template
    {
        /// <summary>
        /// Initializes a new instance of the GH_Download class.
        /// </summary>
        public GH_Download()
          : base("Download", "Download",
              "Download a file from linux to local in windows",
              "1 SSH")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Linux File Paths", "Linux File", "Linux file path", GH_ParamAccess.list);
            pManager[pManager.AddTextParameter("Target local folder", "target folder", "Local target folder in windows", GH_ParamAccess.item, "")].Optional = true;
            pManager.AddBooleanParameter("Run", "Run", "Run", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Status", "Status", "status", GH_ParamAccess.item);
            pManager.AddTextParameter("File Paths", "File Paths", "Path to the files", GH_ParamAccess.list);
            pManager.AddTextParameter("Run", "Run", "Run", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            //Read and parse the input.
            var runTree = new GH_Structure<GH_Boolean>();
            runTree.Append(new GH_Boolean(DA.Fetch<bool>("Run")));
            Params.Output[Params.Output.Count - 1].ClearData();
            DA.SetDataTree(Params.Output.Count - 1, runTree);

            if (!DA.Fetch<bool>("Run"))
                return;

            string localTargetFolder = Path.GetDirectoryName(DA.Fetch<string>("Target local folder"));

            List<string> allFilePaths = DA.FetchList<string>("Linux File Paths");
            List<string> localFilePaths = new List<string>(allFilePaths.Count);

            StringBuilder sb = new StringBuilder();

            foreach (var file in allFilePaths)
            {
                SSH_Helper.Download(file, localTargetFolder, sb);
                localFilePaths.Add(localTargetFolder + "\\" + Path.GetFileName(file));
            }

            DA.SetDataList("File Paths", localFilePaths);
            DA.SetData("Status", sb.ToString());
        }

        

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6C897E25-1EBE-48D8-AD3A-111DB1B2AD90"); }
        }
    }
}