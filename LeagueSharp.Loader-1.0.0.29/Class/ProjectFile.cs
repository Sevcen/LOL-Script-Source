﻿#region LICENSE

// Copyright 2014 LeagueSharp.Loader
// ProjectFile.cs is part of LeagueSharp.Loader.
// 
// LeagueSharp.Loader is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// LeagueSharp.Loader is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with LeagueSharp.Loader. If not, see <http://www.gnu.org/licenses/>.

#endregion

namespace LeagueSharp.Loader.Class
{
    #region

    using System;
    using System.IO;
    using LeagueSharp.Loader.Data;
    using Microsoft.Build.Evaluation;

    #endregion

    [Serializable]
    internal class ProjectFile
    {
        public readonly Project Project;
        private readonly Log _log;

        public ProjectFile(string file, Log log)
        {
            try
            {
                _log = log;

                if (File.Exists(file))
                {
                    ProjectCollection.GlobalProjectCollection.UnloadAllProjects();
                    Project = new Project(file);
                }
            }
            catch (Exception ex)
            {
                Utility.Log(LogStatus.Error, "ProjectFile", string.Format("Error - {0}", ex.Message), _log);
            }
        }

        public bool PrebuildEvent { get; set; }
        public bool PostbuildEvent { get; set; }
        public string Configuration { get; set; }
        public string PlatformTarget { get; set; }
        public bool UpdateReferences { get; set; }
        public string ReferencesPath { get; set; }
        public bool ResetOutputPath { get; set; }

        public void Change()
        {
            try
            {
                if (Project == null)
                {
                    return;
                }
                if (!string.IsNullOrWhiteSpace(Configuration))
                {
                    Project.SetProperty("Configuration", Configuration);
                    Project.Save();
                }
                if (PrebuildEvent)
                {
                    Project.SetProperty("PreBuildEvent", string.Empty);
                }
                if (PostbuildEvent)
                {
                    Project.SetProperty("PostBuildEvent", string.Empty);
                }
                if (!string.IsNullOrWhiteSpace(PlatformTarget))
                {
                    Project.SetProperty("PlatformTarget", PlatformTarget);
                }
                var outputPath = Project.GetProperty("OutputPath");
                if (ResetOutputPath || outputPath == null || string.IsNullOrWhiteSpace(outputPath.EvaluatedValue))
                {
                    Project.SetProperty("OutputPath", "bin\\" + Configuration);
                }
                if (UpdateReferences)
                {
                    foreach (var item in Project.GetItems("Reference"))
                    {
                        if (item == null)
                        {
                            continue;
                        }
                        var hintPath = item.GetMetadata("HintPath");
                        if (hintPath != null && !string.IsNullOrWhiteSpace(hintPath.EvaluatedValue))
                        {
                            item.SetMetadataValue(
                                "HintPath", Path.Combine(ReferencesPath, Path.GetFileName(hintPath.EvaluatedValue)));
                        }
                    }
                }
                Project.Save();
                Utility.Log(LogStatus.Ok, "ProjectFile", string.Format("File Updated - {0}", Project.FullPath), _log);
            }
            catch (Exception ex)
            {
                Utility.Log(LogStatus.Error, "ProjectFile", ex.Message, _log);
            }
        }
    }
}