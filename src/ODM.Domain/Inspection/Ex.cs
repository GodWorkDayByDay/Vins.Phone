using System;
using System.Linq;
using AutoMapper;
using Hdc.Windows.Media.Imaging;
using Microsoft.Practices.ServiceLocation;
using ODM.Domain.Schemas;
using Omu.ValueInjecter;

namespace ODM.Domain.Inspection
{
    public static class Ex
    {
        static Ex()
        {
//            Mapper.CreateMap<Inspection.DefectInfo, DefectInfo>()
//                  .ForMember(x => x.X, o => o.MapFrom(x => x.DefectPosX))
//                  .ForMember(x => x.Y, o => o.MapFrom(x => x.DefectPosY))
//                  .ForMember(x => x.Width, o => o.MapFrom(x => x.DefectWidth))
//                  .ForMember(x => x.Height, o => o.MapFrom(x => x.DefectHeight))
//                  .ForMember(x => x.Index, o => o.MapFrom(x => x.DefectId))
                ;
        }

        public static WorkpieceInfoEntry ToEntry(this WorkpieceInfo workpieceInfo)
        {
            var id = new WorkpieceInfoEntry();
            id.InjectFrom(workpieceInfo);
            return id;
        }

        public static InspectInfo ToEntity(this Hdc.Mv.Inspection.InspectionInfo di)
        {
            var inspectInfo = new InspectInfo()
                              {
                                  Index = di.Index,
                                  HasError = di.HasError,
                                  SurfaceTypeIndex = di.SurfaceTypeIndex,
                                  DefectInfos =  di.DefectInfos.Select(x=>x.ToEntity()).ToList(),
                                  MeasurementInfos =  di.MeasurementInfos.Select(x=>x.ToEntity()).ToList(),
                              };
            return inspectInfo;
        }


        public static ImageInfo ToEntity(this Hdc.Mv.ImageInfo ii)
        {
            var ii2 = new ImageInfo()
            {
                SurfaceTypeIndex = ii.SurfaceTypeIndex,
                Width = ii.PixelWidth,
                Height = ii.PixelHeight,
                BitsPerPixel = ii.BitsPerPixel,
                Buffer = ii.BufferPtr,
            };
            return ii2;
        }

        public static DefectInfo ToEntity(this Hdc.Mv.Inspection.DefectInfo ii)
        {
            var ii2 = new DefectInfo()
            {
                Index = ii.Index,
                TypeCode = ii.TypeCode,
                Type = (DefectType)ii.TypeCode,
                X = ii.X,
                Y = ii.Y,
                Width = ii.Width,
                Height = ii.Height,
                Size = ii.Size,

                XActualValue = ii.X_Real,
                YActualValue = ii.Y_Real,
                WidthActualValue = ii.Width_Real,
                HeightActualValue = ii.Height_Real,
                SizeActualValue = ii.Size_Real,
            };
            return ii2;
        }

        public static MeasurementInfo ToEntity(this Hdc.Mv.Inspection.MeasurementInfo ii)
        {
            var ii2 = new MeasurementInfo()
            {
                Index = ii.Index,
                TypeCode = ii.TypeCode,
                StartPointX = ii.StartPointX,
                StartPointY = ii.StartPointY,
                EndPointX = ii.EndPointX,
                EndPointY = ii.EndPointY,
                Value = ii.Value,
                GroupIndex = ii.GroupIndex,

                StartPointXActualValue = ii.StartPointXActualValue,
                StartPointYActualValue = ii.StartPointYActualValue,
                EndPointXActualValue = ii.EndPointXActualValue,
                EndPointYActualValue = ii.EndPointYActualValue,
                ValueActualValue = ii.ValueActualValue,
            };
            return ii2;
        }


        public static StoredImageInfo SaveImage(this SurfaceInspectInfo surfaceInspectInfo)
        {
            var prefix = DateTime.Now.ToString(
                //                @"yyyy-MM-dd_hh\Hmm\Mss\S") + "_"
                @"yyyy-MM-dd_HH.mm.ss") + "_"
                         + "WI" + surfaceInspectInfo.WorkpieceIndex + "."
                         + "SI" + surfaceInspectInfo.SurfaceTypeIndex + "."
                         + "_";
            var sii = ImageSaveLoadService.StoreImage(
                fileName => surfaceInspectInfo.ImageInfo
                    .ToBitmapSource()
                    .SaveToTiffAsync(fileName), prefix, ".tif");
            sii.SurfaceTypeIndex = surfaceInspectInfo.SurfaceTypeIndex;
            return sii;
        }

        private static IImageSaveLoadService _imageSaveLoadService;

        public static IImageSaveLoadService ImageSaveLoadService
        {
            get
            {
                return _imageSaveLoadService ??
                       (_imageSaveLoadService = ServiceLocator.Current.GetInstance<IImageSaveLoadService>());
            }
        }
    }
}