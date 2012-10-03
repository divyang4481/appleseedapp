﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using FileManager.Models;

namespace FileManager.Controllers {
    public class HomeController : Controller {


        public ActionResult Index() {
            return View();
        }

        /// <summary>
        /// A method to populate a TreeView with directories, subdirectories, etc
        /// </summary>
        /// <param name="dir">The path of the directory</param>
        /// <param name="node">The "master" node, to populate</param>
        public void PopulateTree(string dir, FilesTree node) {
            var directory = new DirectoryInfo(Request.MapPath(dir));

            if (node.children == null && directory.GetDirectories().Length > 0) {
                node.children = new List<FilesTree>();
            }
            // get the information of the directory
            
            // loop through each subdirectory
            foreach (var t in from d in directory.GetDirectories() let dirName = string.Format("{0}/{1}", dir, d.Name) select new FilesTree { attr = new FilesTreeAttribute { id = dirName }, data = d.Name, state = "closed"} into t where node.children != null select t)
            {
                node.children.Add(t); // add the node to the "master" node
            }

            // lastly, loop through each file in the directory, and add these as nodes
            //foreach (var f in directory.GetFiles()) {
            //    // create a new node
            //    var t = new FilesTree {attr = new FilesTreeAttribute {id = f.FullName}, data = f.Name};
            //    // add it to the "master"
            //    node.children.Add(t);
            //}
        }


        [HttpPost]
        public JsonResult GetTreeData()
        {
            const string rootPath = "/Portals";
            var rootNode = new FilesTree { attr = new FilesTreeAttribute { id = rootPath }, data = "Portals" };
            rootNode.attr.id = rootPath;
            PopulateTree(rootPath, rootNode);
            return Json(rootNode);
        }

        

        [HttpPost]
        public JsonResult GetChildreenTree(string dir)
        {
            var rootNode = new FilesTree { attr = new FilesTreeAttribute { id = dir }, data = dir.Substring(1) };
            var rootPath = dir;
            rootNode.attr.id = rootPath;
            PopulateTree(rootPath, rootNode);
            
            return Json(rootNode.children);
        }


        [HttpPost]
        public ActionResult MoveData(string path, string destination) {
            // get the file attributes for file or directory
            var attPath = System.IO.File.GetAttributes(path);

            var attDestination = System.IO.File.GetAttributes(path);

            var fi = new FileInfo(path);

            //detect whether its a directory or file
            if ((attPath & FileAttributes.Directory) == FileAttributes.Directory) {
                if ((attDestination & FileAttributes.Directory) == FileAttributes.Directory) {
                    MoveDirectory(path, destination);
                }
            }
            else {
                System.IO.File.Move(path, destination + "\\" + fi.Name);
            }
            return null;
        }

        [HttpPost]
        public ActionResult CreateFolder(string path, string newname) {
            Directory.CreateDirectory(path + "\\" + newname);
            return null;
        }



        public void MoveDirectory(string source, string target) {
            var stack = new Stack<Folders>();
            stack.Push(new Folders(source, target));

            while (stack.Count > 0) {
                var folders = stack.Pop();
                Directory.CreateDirectory(folders.Target);
                foreach (var file in Directory.GetFiles(folders.Source, "*.*")) {
                    string targetFile = Path.Combine(folders.Target, Path.GetFileName(file));
                    if (System.IO.File.Exists(targetFile)) System.IO.File.Delete(targetFile);
                    System.IO.File.Move(file, targetFile);
                }

                foreach (var folder in Directory.GetDirectories(folders.Source)) {
                    stack.Push(new Folders(folder, Path.Combine(folders.Target, Path.GetFileName(folder))));
                }
            }
            Directory.Delete(source, true);
        }
        public class Folders {
            public string Source { get; private set; }
            public string Target { get; private set; }

            public Folders(string source, string target) {
                Source = source;
                Target = target;
            }
        }

        public ActionResult ViewFilesFromFolder(string folder)
        {

            var directory = new DirectoryInfo(Request.MapPath(folder));
            var folderContent = new FolderContent();

            foreach (var f in directory.GetFiles()) {

                folderContent.Files.Add(f.Name);
            }

            foreach (var f in directory.GetDirectories()) {

                folderContent.Folders.Add(f.Name);
            }


            return View("FilesView", folderContent);
        }


    }
}
