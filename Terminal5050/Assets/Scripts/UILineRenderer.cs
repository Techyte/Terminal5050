using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic
{
     public List<Vector2> points;

     public Color color;
     public float thickness;

     float width;
     float height;
     float unitWidth = 1;
     float unitHeight = 1;
     
     
     protected override void OnPopulateMesh(VertexHelper vh)
     {
          Debug.Log("OnPopulateMesh");
          
          vh.Clear();
          
          width = rectTransform.rect.width;
          height = rectTransform.rect.height;

          if (points.Count <= 2)
          {
               return;
          }

          for (int i = 0; i < points.Count; i++)
          {
               Vector2 point = points[i];
               DrawVerticiesForPoint(point, vh);
          }

          for (int i = 0; i < points.Count; i++)
          {
               int index = i * 2;
               vh.AddTriangle(index + 0, index + 1, index + 3);
               vh.AddTriangle(index + 3, index + 2, index + 0);
          }
          
          base.OnPopulateMesh(vh);
     }

     void DrawVerticiesForPoint(Vector2 point, VertexHelper vh)
     {
          UIVertex vertex = UIVertex.simpleVert;
          vertex.color = color;

          vertex.position = new Vector3(-thickness / 2, 0);
          vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
          vh.AddVert(vertex);
          
          vertex.position = new Vector3(thickness / 2, 0);
          vertex.position +=  new Vector3(unitWidth * point.x, unitHeight * point.y);
          vh.AddVert(vertex);
     }
}