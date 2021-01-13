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

        // data assuming string
        //
        // 0000 - unsigned int - number of elements
        // 0000 - unsigned int - pointer to current element
        // 00 - unsigned int - Length of element handled by the binary writer and reader in LEB128 format
        // bytes - string
        // ...
        // 00 - unsigned int - Length of element handled by the binary writer and reader in LEB128 format
        // bytes - string

        // Index
        // 0000 - unsigned int - number of elements
        // 0000 - unsigned int - pointer to data
        // ...
        // 0000 - unsigned int - pointer to data + 1

        // Data
        // 

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
                // Need to search the index file 

                string data = "test data";
                return ((T)Convert.ChangeType(data, typeof(T)));
            }
            set
            {
                // Need to update the item at the index
                // This is more complex for strings if the new string is longer than the
                // available space from the previous string. Just occred to me that 
                // might be a good idea to store the orinal length or space as new 
                // strings might end of getting shorter and shorter
                
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

                BinaryWriter binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".idx", FileMode.Append));
                binaryWriter.Write(_pointer);  // Write the pointer
                binaryWriter.Close();

                // Need to consider how data is stored
                // so if int, string

                int offset = 0; 
                if (ParameterType == typeof(string))
                {
                    offset = offset + 1 + Convert.ToString(item).Length; // Includes the byte length parameter
                    // ** need to watch this as can be 2 bytes if length is > 127 characters
                    // ** https://en.wikipedia.org/wiki/LEB128
                }

                // Write the data

                binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".bin", FileMode.OpenOrCreate));
                binaryWriter.Seek(0, SeekOrigin.Begin); // Move to start of the file
                _size++;
                binaryWriter.Write(_size);  // Write the size
                _pointer = (UInt16)(_pointer + offset);
                binaryWriter.Write(_pointer);  // Write the pointer
                binaryWriter.Close();

                // Appending will only work if the file is deleted and the updates start again
                // Not sure if this is the best approach.
                // Need to update the 
                // With strings might have to do the write first and then update the pointer.

                binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".bin", FileMode.Append));
                if (ParameterType == typeof(string))
                {
                    string s = Convert.ToString(item);
                    long before = binaryWriter.BaseStream.Position;
                    binaryWriter.Write(s);
                    long after = binaryWriter.BaseStream.Position;

                }
                binaryWriter.Close();
            }
        }

        #endregion
        #region Private

        private void Open(string path, string filename, bool reset)
        {
            string filenamePath = System.IO.Path.Combine(path, filename);
            if ((File.Exists(filenamePath + ".bin") == true) && (reset == false))
            {
                BinaryReader binaryReader = new BinaryReader(new FileStream(filenamePath + ".bin", FileMode.Open));
                _size = binaryReader.ReadUInt16();
                _pointer = binaryReader.ReadUInt16();
                binaryReader.Close();
            }
            else
            {
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
            _pointer = 4;   // Start of the data
            binaryWriter.Write(_size);  // Write the new size
            binaryWriter.Write(_pointer);  // Write the new pointer
            binaryWriter.BaseStream.SetLength(4);
            binaryWriter.Close();

            // Create the index

            binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".idx", FileMode.OpenOrCreate));
            binaryWriter.BaseStream.SetLength(0);
            binaryWriter.Close();

        }

        #endregion
    }
}
