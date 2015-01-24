﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    public interface IGeneralInspector : IDisposable
    {
        void Init();

        void SetImageInfo(HImage imageInfo);

        InspectionResult Inspect(InspectionSchema inspectionSchema);

        CircleSearchingResultCollection SearchCircles(IList<CircleSearchingDefinition> circleSearchingDefinitions);

        EdgeSearchingResultCollection SearchEdges(IList<EdgeSearchingDefinition> edgeSearchingDefinitions);

//        DefectResultCollection SearchDefects(HImage imageInfo);
//
//        DefectResultCollection SearchDefects(HImage imageInfo, HImage mask);

//        HImage FindRegions(HImage imageInfo);
//
//        HImage FindRegions();
    }
}