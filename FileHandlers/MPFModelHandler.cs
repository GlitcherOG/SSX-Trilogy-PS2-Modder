﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSX_Modder.Utilities;

namespace SSX_Modder.FileHandlers
{
    public class MPFModelHandler
    {
        public List<MPFModelHeader> ModelList = new List<MPFModelHeader>();

        public void load(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                //Load Headers
                while (true)
                {
                    MPFModelHeader modelHeader = new MPFModelHeader();

                    modelHeader.U1 = StreamUtil.ReadInt32(stream);
                    modelHeader.SubHeaders = StreamUtil.ReadInt16(stream);
                    modelHeader.HeaderSize = StreamUtil.ReadInt16(stream);
                    modelHeader.FileStart = StreamUtil.ReadInt32(stream);

                    int Test = StreamUtil.ReadInt32(stream);
                    if (Test == 0)
                    {
                        ModelList.Add(modelHeader);
                        break;
                    }
                    else
                    {
                        stream.Position -= 4;
                    }

                    modelHeader.ModelName = StreamUtil.ReadString(stream, 16).Replace("\0", "");
                    modelHeader.DataOffset = StreamUtil.ReadInt32(stream);
                    modelHeader.EntrySize = StreamUtil.ReadInt32(stream);
                    modelHeader.NameOffset = StreamUtil.ReadInt32(stream);
                    modelHeader.U7 = StreamUtil.ReadInt32(stream);
                    modelHeader.U8 = StreamUtil.ReadInt32(stream);
                    modelHeader.U9 = StreamUtil.ReadInt32(stream);
                    modelHeader.U10 = StreamUtil.ReadInt32(stream);
                    modelHeader.U11 = StreamUtil.ReadInt32(stream);
                    modelHeader.U12 = StreamUtil.ReadInt32(stream);
                    modelHeader.U13 = StreamUtil.ReadInt32(stream);
                    modelHeader.U14 = StreamUtil.ReadInt32(stream);
                    modelHeader.U15 = StreamUtil.ReadInt32(stream);
                    modelHeader.U16 = StreamUtil.ReadInt32(stream);

                    modelHeader.U17 = StreamUtil.ReadInt16(stream);
                    modelHeader.U18 = StreamUtil.ReadInt16(stream);
                    modelHeader.U19 = StreamUtil.ReadInt16(stream);
                    modelHeader.U20 = StreamUtil.ReadInt16(stream);
                    modelHeader.BodyObjectsCount = StreamUtil.ReadInt16(stream);
                    modelHeader.U22 = StreamUtil.ReadInt16(stream);
                    modelHeader.U23 = StreamUtil.ReadInt16(stream);
                    modelHeader.U24 = StreamUtil.ReadInt16(stream);
                    ModelList.Add(modelHeader);
                }

                //Read Matrix
                int StartPos = ModelList[0].FileStart;
                for (int i = 0; i < ModelList.Count-1; i++)
                {
                    stream.Position = StartPos + ModelList[i].DataOffset;
                    int EndPos = 0;
                    if(i == ModelList.Count - 2)
                    {
                        EndPos = ((int)stream.Length - StartPos) - ModelList[i].DataOffset;
                    }
                    else
                    {
                        EndPos = ModelList[i + 1].DataOffset - ModelList[i].DataOffset;
                    }

                    MPFModelHeader modelHandler = ModelList[i];
                    modelHandler.Matrix = StreamUtil.ReadBytes(stream, EndPos);
                    RefpackHandler refpackHandler = new RefpackHandler();
                    modelHandler.Matrix = refpackHandler.Decompress(modelHandler.Matrix);
                    ModelList[i] = modelHandler;
                }

                for (int i = 0; i < ModelList.Count-1; i++)
                {
                    Stream streamMatrix = new MemoryStream();
                    var Model = ModelList[i];
                    streamMatrix.Write(ModelList[i].Matrix, 0, ModelList[i].Matrix.Length);

                    //U7 IDK
                    streamMatrix.Position = Model.U7;
                    List<MPFUnkownArray1> TempArrayListU7 = new List<MPFUnkownArray1>();
                    for (int a = 0; a < Model.U17; a++)
                    {
                        MPFUnkownArray1 TempArray = new MPFUnkownArray1();
                        TempArray.Count = StreamUtil.ReadInt32(streamMatrix);
                        TempArray.StartOffset = StreamUtil.ReadInt32(streamMatrix);
                        TempArray.EndOffset = StreamUtil.ReadInt32(streamMatrix);
                        long Position = streamMatrix.Position;

                        //Read Ints
                        TempArray.IntList = new List<int>();
                        streamMatrix.Position = TempArray.StartOffset;
                        for (int b = 0; b < TempArray.Count; b++)
                        {
                            TempArray.IntList.Add(StreamUtil.ReadInt32(streamMatrix));
                        }
                        streamMatrix.Position = Position;
                        TempArrayListU7.Add(TempArray);
                    }
                    Model.U7UnkownArray1 = TempArrayListU7;

                    //U12
                    streamMatrix.Position = Model.U12;
                    List<MPFUnkownArray1> TempArrayListU12 = new List<MPFUnkownArray1>();
                    for (int a = 0; a < 3; a++)
                    {
                        MPFUnkownArray1 TempArray = new MPFUnkownArray1();
                        TempArray.Count = StreamUtil.ReadInt32(streamMatrix);
                        TempArray.StartOffset = StreamUtil.ReadInt32(streamMatrix);
                        long Position = streamMatrix.Position;

                        TempArray.IntList = new List<int>();
                        streamMatrix.Position = TempArray.StartOffset;
                        for (int b = 0; b < TempArray.Count; b++)
                        {
                            TempArray.IntList.Add(StreamUtil.ReadInt32(streamMatrix));
                        }
                        streamMatrix.Position = Position;
                        TempArrayListU12.Add(TempArray);
                    }
                    Model.U12UnkownArray2 = TempArrayListU12;
                    streamMatrix.Dispose();
                    streamMatrix.Close();
                }
            }
        }

        public void Save(string path)
        {
            Stream stream = new MemoryStream();
            List<long> StreamPos = new List<long>();
            for (int i = 0; i < 1; i++)
            {
                StreamUtil.WriteInt32(stream, ModelList[i].U1);
                StreamUtil.WriteInt32(stream, ModelList[i].SubHeaders);
                StreamUtil.WriteInt32(stream, ModelList[i].HeaderSize);
                StreamUtil.WriteInt32(stream, 0);

                if (ModelList[i].ModelName == null)
                {
                    byte[] bytes = new byte[4];
                    StreamUtil.WriteBytes(stream, bytes);
                    break;
                }
                StreamUtil.WriteString(stream, ModelList[i].ModelName, 16);

                StreamPos.Add(stream.Position);
                StreamUtil.WriteInt32(stream, 0);

                StreamUtil.WriteInt32(stream, ModelList[i].EntrySize);
                StreamUtil.WriteInt32(stream, ModelList[i].NameOffset);
                StreamUtil.WriteInt32(stream, ModelList[i].U7);
                StreamUtil.WriteInt32(stream, ModelList[i].U8);
                StreamUtil.WriteInt32(stream, ModelList[i].U9);
                StreamUtil.WriteInt32(stream, ModelList[i].U10);
                StreamUtil.WriteInt32(stream, ModelList[i].U11);
                StreamUtil.WriteInt32(stream, ModelList[i].U12);
                StreamUtil.WriteInt32(stream, ModelList[i].U13);
                StreamUtil.WriteInt32(stream, ModelList[i].U14);
                StreamUtil.WriteInt32(stream, ModelList[i].U15);
                StreamUtil.WriteInt32(stream, ModelList[i].U16);

                StreamUtil.WriteInt16(stream, ModelList[i].U17);
                StreamUtil.WriteInt16(stream, ModelList[i].U18);
                StreamUtil.WriteInt16(stream, ModelList[i].U19);
                StreamUtil.WriteInt16(stream, ModelList[i].U20);
                StreamUtil.WriteInt16(stream, ModelList[i].BodyObjectsCount);
                StreamUtil.WriteInt16(stream, ModelList[i].U22);
                StreamUtil.WriteInt16(stream, ModelList[i].U23);
                StreamUtil.WriteInt16(stream, ModelList[i].U24);
            }

            stream.Position = 8;
            int FileStart = (int)stream.Length;
            StreamUtil.WriteInt32(stream, FileStart);
            stream.Position = stream.Length;

            for (int i = 0; i < 1; i++)
            {
                //Save current pos go back and set start pos
                long CurPos = stream.Position - FileStart;
                stream.Position = StreamPos[i];
                StreamUtil.WriteInt32(stream, (int)CurPos);
                stream.Position = CurPos + FileStart;
            //Write Matrix
                StreamUtil.WriteBytes(stream, ModelList[i].Matrix);
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var file = File.Create(path);
            stream.Position = 0;
            stream.CopyTo(file);
            stream.Dispose();
            file.Close();
        }


        public struct MPFModelHeader
        {
            //GlobalHeader
            public int U1;
            public int SubHeaders;
            public int HeaderSize;
            public int FileStart;
            //Main Header
            public string ModelName;
            public int DataOffset;
            public int EntrySize;
            public int NameOffset; //Offset Start Of something (Rotation Info?)
            public int U7; //Offset Start Of something (After Roation Info?)
            public int U8; //Offset Start Of something (Right After U7)
            public int U9; //Offset Start Of something 
            public int U10; //Blank Guessing Also Offset Start
            public int U11; //Same as U7 (After Rotation Rotation Info)
            public int U12; //After U7 (After Roation Info)
            public int U13;
            public int U14;
            public int U15;
            public int U16;

            //Counts
            public int U17; //Faces Count? (U7)
            public int U18; //!8-20 might be counts related to the bottom
            public int U19;
            public int U20; //Rotation Info Objects?
            public int BodyObjectsCount; //BodyObjects?
            public int U22;
            public int U23;
            public int U24; //VertexCount?

            public byte[] Matrix;

            public List<BodyObjects> bodyObjectsList;
            public List<MPFUnkownArray1> U7UnkownArray1; //Uses U17 As Count
            public List<MPFUnkownArray1> U12UnkownArray2;
            //
        }

        public struct BodyObjects
        {
            public string Name;
            public string Unknown;
            public string Unknown2;
            public string Unknown3;
            public string Unknown4;
            public float Float1;
            public float Float2;
            public float Float3;
        }

        public struct MPFUnkownArray1
        {
            //Header
            public int Count;
            public int StartOffset; 
            public int EndOffset; //Sometimes Used Sometimes Not
            public List<int> IntList;
        }
    }
}
