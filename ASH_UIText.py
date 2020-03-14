import StreamHelper;
import sys;

reload(sys);
sys.setdefaultencoding('utf8');

class ASH_UIText:
	def __init__(self, filepath):
		self.pptr_len = 12;
		self.load_form_raw(filepath);
	
	def load_form_raw(self, filepath):
		f = open(filepath, 'rb');

		self.gameobject_pptr = f.read(self.pptr_len);
		self.enabled = StreamHelper.read_uint32(f);
		self.momoscript_pptr = f.read(self.pptr_len);
		self.name = StreamHelper.read_aligned_string(f);
		
		self.material_pttr = f.read(self.pptr_len);
		self.color_RGBA = f.read(4 * 4);
		self.raycast_target = StreamHelper.read_uint32(f);
		self.unknown_calls_count = StreamHelper.read_int32(f); # must be zero
		self.unknown_typename = StreamHelper.read_aligned_string(f);
		self.fontData = f.read(self.pptr_len + 4 * 11);
		
		self.txt = StreamHelper.read_aligned_string(f);
		
		self.other_data = f.read(StreamHelper.get_unread_data_size(f));
		
		f.close();
		
	def save_to_raw(self, filepath):
		f = open(filepath, 'wb');
	
		f.write(self.gameobject_pptr);
		StreamHelper.write_uint32(f, self.enabled);
		f.write(self.momoscript_pptr);
		StreamHelper.write_aligned_string(f, self.name);
		
		f.write(self.material_pttr);
		f.write(self.color_RGBA);
		StreamHelper.write_uint32(f, self.raycast_target);
		StreamHelper.write_int32(f, self.unknown_calls_count);
		StreamHelper.write_aligned_string(f, self.unknown_typename);
		f.write(self.fontData);

		StreamHelper.write_aligned_string(f, self.txt);
		
		f.write(self.other_data);
		
		f.close();