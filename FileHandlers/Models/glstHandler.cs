﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Schema2;

//https://github.com/vpenades/SharpGLTF/blob/master/src/SharpGLTF.Toolkit/Geometry/readme.md
namespace SSX_Modder.FileHandlers
{

    public class glstHandler
    {
        public static void SaveOGglTF(string Output, SSXMPFModelHandler.MPFModelHeader modelHeader)
        {
            var mesh = new MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(modelHeader.FileName);
            //Make Materials

            List<MaterialBuilder> materialBuilders = new List<MaterialBuilder>();

            for (int i = 0; i < modelHeader.materialDataList.Count; i++)
            {
                var TempVar = modelHeader.materialDataList[i];
                var material1 = new MaterialBuilder(TempVar.Name)
                .WithChannelParam(KnownChannel.BaseColor, KnownProperty.RGBA, new Vector4(TempVar.X, TempVar.Y, TempVar.Z, 1));
                materialBuilders.Add(material1);
            }


            for (int i = 0; i < modelHeader.staticMesh.Count; i++)
            {
                var Data = modelHeader.staticMesh[i];
                int tempInt = Data.ChunkID;
                int MatId = modelHeader.chunks[tempInt].MaterialID;

                for (int b = 0; b < Data.faces.Count; b++)
                {
                    var Face = Data.faces[b];
                    VertexPositionNormal TempPos1 = new VertexPositionNormal();
                    TempPos1.Position.X = Face.V1.X;
                    TempPos1.Position.Y = Face.V1.Y;
                    TempPos1.Position.Z = Face.V1.Z;

                    TempPos1.Normal.X = (float)Face.Normal1.X / 4096f;
                    TempPos1.Normal.Y = (float)Face.Normal1.Y / 4096f;
                    TempPos1.Normal.Z = (float)Face.Normal1.Z / 4096f;

                    VertexPositionNormal TempPos2 = new VertexPositionNormal();
                    TempPos2.Position.X = Face.V2.X;
                    TempPos2.Position.Y = Face.V2.Y;
                    TempPos2.Position.Z = Face.V2.Z;

                    TempPos2.Normal.X = (float)Face.Normal2.X/4096f;
                    TempPos2.Normal.Y = (float)Face.Normal2.Y/4096f;
                    TempPos2.Normal.Z = (float)Face.Normal2.Z/4096f;

                    VertexPositionNormal TempPos3 = new VertexPositionNormal();
                    TempPos3.Position.X = Face.V3.X;
                    TempPos3.Position.Y = Face.V3.Y;
                    TempPos3.Position.Z = Face.V3.Z;

                    TempPos3.Normal.X = (float)Face.Normal3.X / 4096f;
                    TempPos3.Normal.Y = (float)Face.Normal3.Y / 4096f;
                    TempPos3.Normal.Z = (float)Face.Normal3.Z / 4096f;

                    VertexTexture1 TempTexture1 = new VertexTexture1();
                    TempTexture1.TexCoord.X = (float)Face.UV1.X/4096f;
                    TempTexture1.TexCoord.Y = (float)Face.UV1.Y/4096f;

                    VertexTexture1 TempTexture2 = new VertexTexture1();
                    TempTexture2.TexCoord.X = (float)Face.UV2.X / 4096f;
                    TempTexture2.TexCoord.Y = (float)Face.UV2.Y / 4096f;

                    VertexTexture1 TempTexture3 = new VertexTexture1();
                    TempTexture3.TexCoord.X = (float)Face.UV3.X / 4096f;
                    TempTexture3.TexCoord.Y = (float)Face.UV3.Y / 4096f;

                    mesh.UsePrimitive(materialBuilders[MatId]).AddTriangle((TempPos1, TempTexture1), (TempPos2, TempTexture2), (TempPos3, TempTexture3));
                }

            }

            var scene = new SharpGLTF.Scenes.SceneBuilder();

            scene.AddRigidMesh(mesh, Matrix4x4.Identity);

            // save the model in different formats

            var model = scene.ToGltf2();
            model.SaveGLTF(Output);
        }

        public static void SaveTrickyglTF(string Output, TrickyMPFModelHandler Handler)
        {

            var scene = new SharpGLTF.Scenes.SceneBuilder();

            for (int ax = 0; ax < Handler.ModelList.Count; ax++)
            {
                var modelHeader = Handler.ModelList[ax];

                var mesh = new MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(modelHeader.FileName);

                List<MaterialBuilder> materialBuilders = new List<MaterialBuilder>();

                for (int i = 0; i < modelHeader.materialDatas.Count; i++)
                {
                    var TempVar = modelHeader.materialDatas[i];
                    var material1 = new MaterialBuilder(TempVar.MainTexture)
                    .WithChannelParam(KnownChannel.BaseColor, KnownProperty.RGBA, new Vector4(TempVar.R, TempVar.G, TempVar.B, 1));
                    materialBuilders.Add(material1);
                }


                for (int i = 0; i < modelHeader.staticMesh.Count; i++)
                {
                    var Data = modelHeader.staticMesh[i];
                    int tempInt = Data.ChunkID;
                    int MatId = modelHeader.MeshGroups[tempInt].MaterialID;

                    for (int b = 0; b < Data.faces.Count; b++)
                    {
                        var Face = Data.faces[b];
                        VertexPositionNormal TempPos1 = new VertexPositionNormal();
                        TempPos1.Position.X = Face.V1.X;
                        TempPos1.Position.Y = Face.V1.Y;
                        TempPos1.Position.Z = Face.V1.Z;

                        TempPos1.Normal.X = (float)Face.Normal1.X / 4096f;
                        TempPos1.Normal.Y = (float)Face.Normal1.Y / 4096f;
                        TempPos1.Normal.Z = (float)Face.Normal1.Z / 4096f;

                        VertexPositionNormal TempPos2 = new VertexPositionNormal();
                        TempPos2.Position.X = Face.V2.X;
                        TempPos2.Position.Y = Face.V2.Y;
                        TempPos2.Position.Z = Face.V2.Z;

                        TempPos2.Normal.X = (float)Face.Normal2.X / 4096f;
                        TempPos2.Normal.Y = (float)Face.Normal2.Y / 4096f;
                        TempPos2.Normal.Z = (float)Face.Normal2.Z / 4096f;

                        VertexPositionNormal TempPos3 = new VertexPositionNormal();
                        TempPos3.Position.X = Face.V3.X;
                        TempPos3.Position.Y = Face.V3.Y;
                        TempPos3.Position.Z = Face.V3.Z;

                        TempPos3.Normal.X = (float)Face.Normal3.X / 4096f;
                        TempPos3.Normal.Y = (float)Face.Normal3.Y / 4096f;
                        TempPos3.Normal.Z = (float)Face.Normal3.Z / 4096f;

                        VertexTexture1 TempTexture1 = new VertexTexture1();
                        TempTexture1.TexCoord.X = (float)Face.UV1.X / 4096f;
                        TempTexture1.TexCoord.Y = (float)Face.UV1.Y / 4096f;

                        VertexTexture1 TempTexture2 = new VertexTexture1();
                        TempTexture2.TexCoord.X = (float)Face.UV2.X / 4096f;
                        TempTexture2.TexCoord.Y = (float)Face.UV2.Y / 4096f;

                        VertexTexture1 TempTexture3 = new VertexTexture1();
                        TempTexture3.TexCoord.X = (float)Face.UV3.X / 4096f;
                        TempTexture3.TexCoord.Y = (float)Face.UV3.Y / 4096f;

                        mesh.UsePrimitive(materialBuilders[MatId]).AddTriangle((TempPos1, TempTexture1), (TempPos2, TempTexture2), (TempPos3, TempTexture3));
                    }

                }

                scene.AddRigidMesh(mesh, Matrix4x4.Identity);
            }

            // save the model in different formats

            var model = scene.ToGltf2();
            model.SaveGLTF(Output);
        }
    }
}
