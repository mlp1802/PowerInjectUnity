using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System.IO;
namespace PowerInject
{
    public class Utils
    {

        public static void print(String str)
        {
            return;
            /*StreamWriter w = File.AppendText("c:/temp/test.txt");
            w.Write(str + "\n");
            w.Flush();
            w.Close();*/
        }
        public static T getFirstComponentInParent<T>(GameObject g)
        {
            if (object.Equals(g, default(T)))
            {
                return default(T);
            }
            else
            {
                var c = g.GetComponent<T>();
                if (c == null)
                {

                    var parentTransform = g.transform.parent;
                    if (!object.Equals(parentTransform, default(T)))
                    {
                        return getFirstComponentInParent<T>(parentTransform.gameObject);
                    }
                    else
                    {
                        return default(T);
                    }

                }
                else
                {
                    return c;
                }

            }
        }
        public static List<GameObject> getChildren(GameObject g)
        {
            var n = g.transform.childCount;
            var children = new List<GameObject>();
            for (int i = 0; i < n; i++)
            {
                children.Add(g.transform.GetChild(i).gameObject);

            }
            return children;
        }
    }
}