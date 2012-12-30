﻿using System;
using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Script;
using System.ComponentModel;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using AjaxControlToolkit;
using System.Text;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;

#region [ Resources ]

[assembly: System.Web.UI.WebResource("LineChart.LineChart.js", "application/x-javascript")]
[assembly: System.Web.UI.WebResource("LineChart.LineChart.debug.js", "application/x-javascript")]
[assembly: WebResource("LineChart.LineChart.css", "text/css", PerformSubstitution = true)]

#endregion

namespace AjaxControlToolkit
{

    /// <summary>
    /// AjaxFileUpload enables you to upload multiple files to a server. Url of uploaded file can be passed
    /// back to client to use e.g. to display preview of image.
    /// </summary>
    [Designer("AjaxControlToolkit.LineChartDesigner, AjaxControlToolkit")]
    [RequiredScript(typeof(CommonToolkitScripts))]
    [ClientCssResource("LineChart.LineChart.css")]
    [ClientScriptResource("Sys.Extended.UI.LineChart", "LineChart.LineChart.js")]
    public class LineChart : ScriptControlBase
    {
        private LineChartSeriesCollection lineChartSeriesList = null;

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new AjaxFileUpload.
        /// </summary>
        public LineChart()
            : base(true, HtmlTextWriterTag.Div)
        {
        }

        #endregion

        #region [Private Properties]
        /// <summary>
        /// Gets whether control is in design mode or not.
        /// </summary>
        private bool IsDesignMode
        {
            get
            {
                return (HttpContext.Current == null);
            }
        }

        #endregion

        #region [ Public Properties ]

        /// <summary>
        /// Width of Line Chart.
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue(null)]
        [ClientPropertyName("chartWidth")]
        public string ChartWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Height of Line Chart. 
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue(null)]
        [ClientPropertyName("chartHeight")]
        public string ChartHeight
        {
            get;
            set;
        }

        /// <summary>
        /// Title of Line Chart. 
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("chartTitle")]
        public string ChartTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Categories Axis of Line Chart. 
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("categoriesAxis")]
        public string CategoriesAxis
        {
            get;
            set;
        }
        
        /// <summary>
        /// Provide list of series to client side. Need help from Series property 
        /// for designer experience support, cause Editor always blocks the property
        /// ability to provide values to client side as ExtenderControlProperty on run time.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ExtenderControlProperty(true, true)]
        public LineChartSeriesCollection ClientSeries
        {
            get
            {
                return lineChartSeriesList;
            }
        }

        /// <summary>
        /// defines series related to Line chart.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(null)]
        [NotifyParentProperty(true)]
        [Editor(typeof(LineChartSeriesCollectionEditor), typeof(UITypeEditor))]
        public LineChartSeriesCollection Series
        {
            get
            {
                if (lineChartSeriesList == null)
                    lineChartSeriesList = new LineChartSeriesCollection();
                return lineChartSeriesList;
            }
        }

        /// <summary>
        /// Type of Line Chart. 
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue(LineChartType.Basic)]
        [ClientPropertyName("chartType")]
        public LineChartType ChartType
        {
            get;
            set;
        }

        /// <summary>
        /// CSS/Theme file name of Line Chart. 
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("LineChart")]
        [ClientPropertyName("theme")]
        public string Theme
        {
            get;
            set;
        }

        /// <summary>
        /// Number of lines on the Value Axis. 
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue(9)]
        [ClientPropertyName("valueAxisLines")]
        public int ValueAxisLines
        {
            get;
            set;
        }

        /// <summary>
        /// Title's text color of Bar Chart. 
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("chartTitleColor")]
        public string ChartTitleColor
        {
            get;
            set;
        }

        /// <summary>
        /// Color of background lines of Value Axis of Bar Chart. 
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("valueAxisLineColor")]
        public string ValueAxisLineColor
        {
            get;
            set;
        }

        /// <summary>
        /// Color of background lines of Catgory Axis of Bar Chart. 
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("categoryAxisLineColor")]
        public string CategoryAxisLineColor
        {
            get;
            set;
        }

        /// <summary>
        /// Color of Base Lines of Bar Chart. 
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("baseLineColor")]
        public string BaseLineColor
        {
            get;
            set;
        }

        #endregion

        #region [ Members ]

        /// <summary>
        /// On Init to validate data.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!IsDesignMode)
            {
                foreach (LineChartSeries lineChartSeries in Series)
                {
                    if (lineChartSeries.Name == null || lineChartSeries.Name.Trim() == "")
                    {
                        throw new Exception("Name is missing in the LineChartSeries. Please provide a name in the LineChartSeries.");
                    }
                }
            }
        }

        /// <summary>
        /// CreateChilds call to create child controls for Line Chart.
        /// </summary>
        internal void CreateChilds()
        {
            this.Controls.Clear();
            this.CreateChildControls();
        }

        /// <summary>
        /// CreateChildControls creates html controls for a Line Chart control.
        /// </summary>
        protected override void CreateChildControls()
        {
            GenerateHtmlInputControls();
        }


        /// <summary>
        /// GenerateHtmlInputControls creates parent div for Line Chart control.
        /// </summary>
        /// <returns>Return the client id of parent div that contains all other html controls.</returns>
        protected string GenerateHtmlInputControls()
        {
            HtmlGenericControl parent = new HtmlGenericControl("div");
            parent.ID = "_ParentDiv";            
            parent.Attributes.Add("style", "border-style:solid; border-width:1px;");
            Controls.Add(parent);

            return parent.InnerHtml;
        }

        /// <summary>
        /// DescribeComponent creates propreties in ScriptComponentDescriptor for child controls in Line Chart
        /// </summary>
        /// <param name="descriptor">Descriptor object which will accpet server components to convert in client script.</param>
        protected override void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            base.DescribeComponent(descriptor);
            if (!IsDesignMode)
            {
                
            }
        }

        #endregion
       
    }
}
