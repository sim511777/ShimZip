using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimZip {
   class QuakeBsp {
   }

   enum LumpType {
      Planes,
      Nodes,
      Clipnodes,
      Leafs,
      Verts,
      Faces,
      Marksurfaces,
      Texinfo,
      Edges,
      Surfedges,
      Textures,
      Miptex,
      Lighting,
      Visibility,
      Num,
   }

   class Lump {
      public int offset;
      int length;
      public Lump(int offset, int length) {
         this.offset = offset;
         this.length = length;
      }
   }

   class BspHeader {
      public int version;
      public Lump[] lumps = new Lump[(int)LumpType.Num];
   }
}
