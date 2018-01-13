using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine.UI;
using UniRx;
using UnityEngine.EventSystems;

namespace Approximator
{
    public class ScheduleView : View
    {
        [Inject]
        public IEventDispatcher Dispatcher { get; private set; }
        ScheduleConfig config;

        [SerializeField]
        GraphicRaycaster raycaster;
        [SerializeField]
        EventSystem eventSystem;
        PointerEventData eventData;

        internal const string TOUCH = "touch";

        List<Line> Axis;
        Vector3 StartPosition;
        Vector3 EndXPosition;
        Vector3 EndYPosition;
        float xLen;
        float yLen;
        float xStep;
        float yStep;
        List<Dot> points;
        Line ols;
        Line lagrange;

        internal void Init(ScheduleConfig config)
        {
            this.config = config;
            Line xAxis = new Line();
            xAxis.Mat = config.ScheduleMaterial;
            xAxis.Points = new List<Vector3>();
            StartPosition = WorldPoint(new Vector3(config.Offset, config.Offset));
            xAxis.Points.Add(StartPosition);
            xAxis.Points.Add(WorldPoint(new Vector3(Screen.width - config.Offset, config.Offset)));
            xLen = WorldPoint(Screen.width - config.Offset,0).x - WorldPoint(config.Offset,0).x;

            Line yAxis = new Line();
            yAxis.Mat = config.ScheduleMaterial;
            yAxis.Points = new List<Vector3>();
            yAxis.Points.Add(StartPosition);
            yAxis.Points.Add(WorldPoint(new Vector3(config.Offset, Screen.height - config.Offset)));
            yLen = WorldPoint(0f, Screen.height - config.Offset).y - WorldPoint(0f, config.Offset).y;
            Axis = new List<Line>();
            Axis.Add(xAxis);
            Axis.Add(yAxis);

            xStep = xLen / config.XSteps;
            float offset = config.DashLength / 2f;
            for (int i = 1; i < config.XSteps; ++i)
            {
                float x = StartPosition.x + xStep * i;
                float y = StartPosition.y;
                Line line = new Line();
                line.Mat = config.ScheduleMaterial;
                line.Points = new List<Vector3>();
                line.Points.Add(new Vector3(x, y - offset));
                line.Points.Add(new Vector3(x, y + offset));
                Axis.Add(line);
            }
            yStep = yLen / config.YSteps;
            for(int i = 1; i < config.YSteps; ++i)
            {
                float x = StartPosition.x;
                float y = StartPosition.y + yStep * i;
                Line line = new Line();
                line.Mat = config.ScheduleMaterial;
                line.Points = new List<Vector3>();
                line.Points.Add(new Vector3(x - offset, y));
                line.Points.Add(new Vector3(x + offset, y));
                Axis.Add(line);
            }
            Camera.onPostRender += OnPostRender;
        }
        void OnPostRender(Camera cam)
        {
            if (Axis != null)
            {
                foreach (Line line in Axis)
                    line.Render();
            }
            if(points != null)
            {
                foreach (Dot dot in points)
                    dot.Render();
            }
            if (ols != null)
                ols.Render();
            if (lagrange != null)
                lagrange.Render();
        }

        Vector3 WorldPoint(Vector3 screen)
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(screen.x, screen.y, Camera.main.nearClipPlane - Camera.main.transform.position.z));
        }
        Vector3 WorldPoint(float x, float y)
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0));
        }
        internal void OnTouch(Vector3 pos)
        {
            Vector3 worldPos = WorldPoint(pos);
            float x = (worldPos.x - StartPosition.x) / xStep;
            if (x < 0)
                return;
            int exactX = (int)x;
            if (x - exactX >= 0.5f)
                ++exactX;
            if(exactX > 0 && exactX < config.XSteps - 1)
            {
                float y = (worldPos.y - StartPosition.y) / yStep;
                Point p = new Point { X = exactX, Y = y };
                Dispatcher.Dispatch(TOUCH, p);
            }
        }
        internal void UpdatePoints(ReactiveDictionary<int, float> dots)
        {
            points = new List<Dot>();
            foreach(int x in dots.Keys)
            {
                Dot dot = new Dot();
                dot.Point = new Point();
                dot.Point.X = StartPosition.x + xStep * x;
                dot.Point.Y = dots[x] * yStep + StartPosition.y;
                dot.Mat = config.PointsMaterial;
                dot.Radius = config.PointRadius;
                points.Add(dot);
            }
        }
        internal void UpdateOLSSchedule(MainModel.Calculator func)
        {
            if (config != null)
                UpdateSchedule(func, out ols, config.OLSMaterial);
        }
        internal void UpdateLagrangeSchedule(MainModel.Calculator func)
        {
            if(config != null)
                UpdateSchedule(func, out lagrange, config.LagrangeMaterial);
        }
        void UpdateSchedule(MainModel.Calculator func, out Line sch, Material mat)
        {
            sch = new Line();
            if (func == null)
                return;
            sch.Mat = mat;
            sch.Points = new List<Vector3>();
            float step = xStep / config.DrawingFrequency;
            float endPoint = StartPosition.x + xStep * (config.XSteps - 1);
            for (float realX = StartPosition.x; realX < endPoint; realX += step)
            {
                float tableX = TableX(realX);
                float realY = RealY(func(tableX));
                sch.Points.Add(new Vector3(realX, realY));
            }
        }
        float TableX(float realX)
        {
            return (realX - StartPosition.x) / xStep;
        }
        float RealX(float tableX)
        {
            return StartPosition.x + xStep * tableX;
        }
        float TableY(float realY)
        {
            return (realY - StartPosition.y) / yStep;
        }
        float RealY(float tableY)
        {
            return StartPosition.y + yStep * tableY;
        }
        public class Point
        {
            public float X { get; set; }
            public float Y { get; set; }
        }
        class Line
        {
            public List<Vector3> Points { get; set; }
            public Material Mat { get; set; }
            public void Render()
            {
                for (int i = 0; i < Points.Count - 1; ++i)
                {
                    GL.Begin(GL.LINES);
                    Mat.SetPass(0);
                    GL.Color(Mat.color);
                    GL.Vertex(Points[i]);
                    GL.Vertex(Points[i + 1]);
                    GL.End();
                }
            }
        }        
        class Dot
        {
            public Point Point { get; set; }
            public Material Mat { get; set; }
            public float Radius { get; set; }
            public void Render()
            {
                GL.Begin(GL.QUADS);
                Mat.SetPass(0);
                GL.Color(Mat.color);
                GL.Vertex(new Vector3(Point.X - Radius / 2f, Point.Y + Radius / 2f));
                GL.Vertex(new Vector3(Point.X + Radius / 2f, Point.Y + Radius / 2f));
                GL.Vertex(new Vector3(Point.X + Radius / 2f, Point.Y - Radius / 2f));
                GL.Vertex(new Vector3(Point.X - Radius / 2f, Point.Y - Radius / 2f));
                GL.End();
            }
        }
    }
}