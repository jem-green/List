using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace List
{
    class PersistentList<T>
    {
        // Create a list of values that is actually a file
        // Need to consider locking as read and writes may conflict
        // Need to understand what thread safe means fully
        // Start simple and just have it as a int, string dictionary
        // assume that the file is stored with the exe
        // will need some form of index as 

        // Header
        //
        // 00 - unsigned int16 - number of elements
        // 00 - unsigned int16 - pointer to current element
        //
        // Data
        //
        // 00 - unsigned int16 - orginal length
        // 00 - leb128 - Length of element handled by the binary writer and reader in LEB128 format
        // bytes - string
        // ...
        // 00 - leb128 - Length of element handled by the binary writer and reader in LEB128 format
        // bytes - string
        //
        // Index
        //
        // 00 - unsigned int16 - pointer to data
        // ...
        // 00 - unsigned int16 - pointer to data + 1 

        #region Variables

        string _path = "";
        string _name = "PersistentList";
        // Lets assume we will add the extension automatically but filename is not correct 

        readonly object _lockObject = new Object();
        UInt16 _size;
        UInt16 _pointer;

        #endregion
        #region Constructors

        public PersistentList()
        {
            Open(_path, _name, false);
        }

        public PersistentList(bool reset)
        {
            Open(_path, _name, reset);
        }

        public PersistentList(string filename)
        {
            _name = filename;
            Open(_path, _name, false);
        }

        public PersistentList(string path, string filename)
        {
            _name = filename;
            _path = path;
            Open(_path, _name, false);
        }
        public PersistentList(string path, string filename, bool reset)
        {
            _name = filename;
            _path = path;
            Open(_path, _name, reset);
        }

        #endregion
        #region Proprties

        public int Count
        {
            get
            {
                return (_size);
            }
        }

        public string Path
        {
            get
            {
                return (_path);
            }
            set
            {
                _path = value;
            }
        }

        public string Name
        {
            get
            {
                return (_name);
            }
            set
            {
                _name = value;
            }
        }

        // Make the indexer property.
        public T this[int index]
        {
            get
            {
                object data;
                lock (_lockObject)
                {
                    if (index < _size)
                    {
                        Type ParameterType = typeof(T);
                        string filenamePath = System.IO.Path.Combine(_path, _name);
                        // Need to search the index file

                        BinaryReader indexReader = new BinaryReader(new FileStream(filenamePath + ".idx", FileMode.Open));
                        BinaryReader binaryReader = new BinaryReader(new FileStream(filenamePath + ".bin", FileMode.Open));
                        indexReader.BaseStream.Seek(index * 4, SeekOrigin.Begin);                               // Get the pointer from the index file
                        UInt16 pointer = indexReader.ReadUInt16();                                              // Reader the pointer from the index file
                        binaryReader.BaseStream.Seek(pointer, SeekOrigin.Begin);                                // Move to the correct location in the data file
                        if (ParameterType == typeof(string))
                        {
                            data = binaryReader.ReadString();
                        }
                        else
                        {   
                            data = default(T);
                        }
                        binaryReader.Close();
                        indexReader.Close();
                        return ((T)Convert.ChangeType(data, typeof(T)));
                    }
                    else
                    {
                        throw new IndexOutOfRangeException();
                    }
                }
            }

            set
            {
                // Need to update the item at the index
                // This is more complex for strings if the new string is longer than the
                // available space from the previous string. Just occred to me that 
                // might be a good idea to store the orinal length or space as new 
                // strings might end of getting shorter and shorter

                lock (_lockObject)
                {
                    if (index < _size)
                    {
                        Type ParameterType = typeof(T);
                        string filenamePath = System.IO.Path.Combine(_path, _name);

                        BinaryReader indexReader = new BinaryReader(new FileStream(filenamePath + ".idx", FileMode.Open));
                        indexReader.BaseStream.Seek(index * 4, SeekOrigin.Begin);                               // Get the index pointer
                        UInt16 pointer = indexReader.ReadUInt16();                                              // Read the pointer from the index file
                        UInt16 length = indexReader.ReadUInt16();                                               // Read the length from the index file
                        indexReader.Close();

                        BinaryWriter binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".bin", FileMode.OpenOrCreate));
                        if (ParameterType == typeof(string))
                        {
                            int offset = 0;
                            if (length > value.ToString().Length)
                            {
                                binaryWriter.Seek(pointer, SeekOrigin.Begin);
                                string s = Convert.ToString(value);
                                binaryWriter.Write(s);
                            }
                            else
                            {
                                // Need to write to new location and update the index
                                length = (UInt16)Convert.ToString(value).Length;
                                offset = offset + LEB128.Size(length) + length; // Includes the byte length parameter
                                                                                // ** need to watch this as can be 2 bytes if length is > 127 characters

                                BinaryWriter indexWriter = new BinaryWriter(new FileStream(filenamePath + ".idx", FileMode.Open));
                                indexWriter.Seek(index * 4, SeekOrigin.Begin);   // Get the index pointer
                                indexWriter.Write(_pointer);
                                indexWriter.Close();

                                binaryWriter.Seek(0, SeekOrigin.Begin);     // Move to start of the file
                                binaryWriter.Write(_size);                  // Write the size
                                _pointer = (UInt16)(_pointer + offset);     //
                                binaryWriter.Write(_pointer);               // Write the pointer
                                binaryWriter.Close();

                                // Write the data

                                // Appending will only work if the file is deleated and the updates start again
                                // Not sure if this is the best approach.
                                // With strings might have to do the write first and then update the pointer.

                                binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".bin", FileMode.Append));
                                string s = Convert.ToString(value);
                                binaryWriter.Write(s);
                            }
                        }
                        else
                        {
                            // Test
                        }
                        binaryWriter.Close();
                    }
                    else
                    {
                        throw new IndexOutOfRangeException();
                    }
                }
            }
        }

        #endregion
        #region Methods

        public void Clear()
        {
            lock (_lockObject)
            {
                Reset(_path, _name);
            }
        }

        /// <summary>
        /// Add a new item at the end of the list
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            string filenamePath = System.IO.Path.Combine(_path, _name);
            lock (_lockObject)
            {
                Type ParameterType = typeof(T);

                // append the new pointer the index file

                BinaryWriter indexWriter = new BinaryWriter(new FileStream(filenamePath + ".idx", FileMode.Append));
                indexWriter.Write(_pointer);  // Write the pointer of the previous location

                // Need to consider how data is stored
                // so if int, string
                // calculate the new pointers

                int offset = 0;
                if (ParameterType == typeof(string))
                {
                    UInt16 length = (UInt16) Convert.ToString(item).Length;
                    offset = offset + LEB128.Size(length) + length; // Includes the byte length parameter
                                                                    // ** need to watch this as can be 2 bytes if length is > 127 characters
                                                                    // ** https://en.wikipedia.org/wiki/LEB128
                    indexWriter.Write(length);
                }
                indexWriter.Close();

                // Write the header

                BinaryWriter binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".bin", FileMode.OpenOrCreate));
                binaryWriter.Seek(0, SeekOrigin.Begin); // Move to start of the file
                _size++;
                binaryWriter.Write(_size);                  // Write the size
                _pointer = (UInt16)(_pointer + offset);     //
                binaryWriter.Write(_pointer);               // Write the pointer
                binaryWriter.Close();

                // Write the data

                // Appending will only work if the file is deleated and the updates start again
                // Not sure if this is the best approach.
                // With strings might have to do the write first and then update the pointer.

                binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".bin", FileMode.Append));
                if (ParameterType == typeof(string))
                {
                    string s = Convert.ToString(item);
                    binaryWriter.Write(s);
                }
                binaryWriter.Close();
            }
        }

        /// <summary>
        /// Add a new item at the end of the list
        /// </summary>
        /// <param name="item"></param>
        public void Remove(T item)
        {
            string filenamePath = System.IO.Path.Combine(_path, _name);

            lock (_lockObject)
            {
                Type ParameterType = typeof(T);

                // Logic is probably to open the index
                // work through this and identify the data position in the file (note zero means that data is delted)
                // read the data
                // check if the data matches
                // remove the data
                // update the index file by removing the refernce

                object data;
                BinaryReader binaryReader = new BinaryReader(new FileStream(filenamePath + ".bin", FileMode.Open));
                BinaryReader indexReader = new BinaryReader(new FileStream(filenamePath + ".idx", FileMode.Open));
                int index = -1;
                for (int counter = 0; counter < _size; counter++)
                {
                    indexReader.BaseStream.Seek(counter * 4, SeekOrigin.Begin);                               // Get the index pointer
                    UInt16 pointer = indexReader.ReadUInt16();                                              // Read the pointer from the index file
                    UInt16 length = indexReader.ReadUInt16();

                    binaryReader.BaseStream.Seek(pointer, SeekOrigin.Begin);                                // Move to the correct location in the data file
                    if (ParameterType == typeof(string))
                    {
                        data = binaryReader.ReadString();
                        if ((string)data == (string)Convert.ChangeType(item, typeof(string)))
                        {
                            index = counter;
                            // Need to store index, pointer
                            break;
                        }
                    }
                    else
                    {
                        data = default(T);
                    }               
                }
                binaryReader.Close();
                indexReader.Close();

                // Write the header

                BinaryWriter binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".bin", FileMode.OpenOrCreate));
                binaryWriter.Seek(0, SeekOrigin.Begin); // Move to start of the file
                _size--;
                binaryWriter.Write(_size);                  // Write the size
                binaryWriter.Close();

                // Overwrite the index

                FileStream stream = new FileStream(filenamePath + ".idx", FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                indexReader = new BinaryReader(stream);
                BinaryWriter indexWriter = new BinaryWriter(stream);

                // copy the 
                for (int counter = index; counter < _size; counter ++)
                {
                    indexReader.BaseStream.Seek((counter + 1) * 4, SeekOrigin.Begin); // Move to location of the index
                    UInt16 pointer = indexReader.ReadUInt16();                                              // Read the pointer from the index file
                    UInt16 length = indexReader.ReadUInt16();
                    indexWriter.Seek(counter * 4, SeekOrigin.Begin); // Move to location of the index
                    indexWriter.Write(pointer);
                    indexWriter.Write(length);
                }
                indexWriter.BaseStream.SetLength(_size * 4);    // Trim the file as Add uses append
                indexWriter.Close();
                indexReader.Close();

            }
        }

        #endregion
        #region Private

        private void Open(string path, string filename, bool reset)
        {
            string filenamePath = System.IO.Path.Combine(path, filename);
            if ((File.Exists(filenamePath + ".bin") == true) && (reset == false))
            {
                // Assume we only need to read the data and not the index
                BinaryReader binaryReader = new BinaryReader(new FileStream(filenamePath + ".bin", FileMode.Open));
                _size = binaryReader.ReadUInt16();
                _pointer = binaryReader.ReadUInt16();
                binaryReader.Close();
            }
            else
            {
                // Need to delete both data and index
                File.Delete(filenamePath + ".bin");
                File.Delete(filenamePath + ".idx");
                Reset(path, filename);
            }
        }

        private void Reset(string path, string filename)
        {
            // Reset the file
            string filenamePath = System.IO.Path.Combine(path, filename);
            BinaryWriter binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".bin", FileMode.OpenOrCreate));
            binaryWriter.Seek(0, SeekOrigin.Begin); // Move to start of the file
            _size = 0;
            _pointer = 4;                           // Start of the data 2 x 16 bit
            binaryWriter.Write(_size);              // Write the new size
            binaryWriter.Write(_pointer);           // Write the new pointer
            binaryWriter.BaseStream.SetLength(4);   // Fix the size as we are resetting
            binaryWriter.Close();

            // Create the index

            binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".idx", FileMode.OpenOrCreate));
            binaryWriter.BaseStream.SetLength(0);
            binaryWriter.Close();

        }

        #endregion
    }

    public static class LEB128
    {
        public static byte[] Encode(int value)
        {
            byte[] data = new byte[5];  // Assume 32 bit max as its an int32
            int size = 0;
            do
            {
                byte byt = (byte)(value & 0x7f);
                value >>= 7;
                if (value != 0)
                {
                    byt = (byte)(byt | 128);
                }
                data[size] = byt;
                size = size + 1;
            } while (value != 0);
            return (data);
        }

        public static int Size(int value)
        {
            int size = 0;
            do
            {
                byte byt = (byte)(value & 0x7f);
                value >>= 7;
                size = size + 1;
            } while (value != 0);
            return (size);
        }
    }
}

