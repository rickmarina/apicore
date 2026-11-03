using System;
using System.Collections.Generic;
using System.Text;

namespace com.rorisoft.structures.tree
{
    public class TrieTree
    {
        public class Node
        {
            public string? value;
            public Dictionary<char, Node> nodos;
            public bool isEnd;

            public Node()
            {
                nodos = new Dictionary<char, Node>();
                isEnd = false; 
            }
        }

        Node root;

        public TrieTree()
        {
            this.root = new Node();
        }

        public void InitTree(string[] words)
        {
            foreach (var w in words)
            {
                Insert(w, w);
            }
        }

        public void Insert(string word, string value)
        {
            Node nodo = this.root;
            foreach (var c in word)
            {
                if (!nodo.nodos.ContainsKey(c))
                {
                    Node nuevoNodo = new Node();
                    nodo.nodos.Add(c, nuevoNodo);
                    nodo = nuevoNodo;
                } else
                {
                    nodo = nodo.nodos[c];
                }
            }
            nodo.isEnd = true; //marcar el último nodo como hoja 
            nodo.value = value;
        }

        public Node GetRoot()
        {
            return this.root;
        }

        //Método que permite buscar una palabra entera
        // true -> la palabra clave está insertada en el árbol 
        // false -> no existe la palabra clave 
        public bool ExistWord(string key)
        {
            Node nodo = this.root;
            foreach (var c in key)
            {
                if (!nodo.nodos.ContainsKey(c))
                {
                    return false; 
                }

                nodo = nodo.nodos[c];

            }
            return nodo != null && nodo.isEnd;
        }

        public Node? GetLastNode(string prefix)
        {
            Node nodo = this.root;
            foreach ( var c in prefix)
            {
                if (!nodo.nodos.ContainsKey(c))
                {
                    return null;
                } else
                {
                    nodo = nodo.nodos[c];
                }
            }
            return nodo;
        }

        public IEnumerable<string> GetSuggestions(string prefix, int max)
        {
            List<string> results = new List<string>();
            int total = 0;

            Node? actualNode = GetLastNode(prefix);

            results = GetChildWords(actualNode, results, max, ref total);

            return results; 
        }

        public List<string> GetChildWords(Node? n, List<string> results, int max, ref int total)
        {
            if (n is null)
                return [];

            if (n.isEnd && total < max)
            {
                if (!string.IsNullOrEmpty(n.value)) {
                    results.Add(n.value);
                    total++;
                }
            }
            if (total < max) { 
                foreach (var item in n.nodos)
                {
                    GetChildWords(item.Value, results, max, ref total);
                }
            }

            return results; 
        }

    }





}
