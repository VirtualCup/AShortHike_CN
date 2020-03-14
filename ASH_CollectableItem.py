import StreamHelper;
import sys;

reload(sys);
sys.setdefaultencoding('utf8');

class ASH_CollectableItem:
	def __init__(self, filepath):
		self.pptr_len = 12;
		self.load_form_raw(filepath);
	
	def load_form_raw(self, filepath):
		f = open(filepath, 'rb');

		self.gameobject_pptr = f.read(self.pptr_len);
		self.enabled = StreamHelper.read_uint32(f);
		self.momoscript_pptr = f.read(self.pptr_len);
		self.name = StreamHelper.read_aligned_string(f);
		
		self.readable_name = StreamHelper.read_aligned_string(f);
		self.readable_name_plural = StreamHelper.read_aligned_string(f);
		self.desicription = StreamHelper.read_aligned_string(f);
		
		self.some_data = f.read(self.pptr_len + 4 * 5 + self.pptr_len);
		self.yarn_node_title = StreamHelper.read_aligned_string(f);

		self.other_data = f.read(StreamHelper.get_unread_data_size(f));
		
		f.close();
		
	def save_to_raw(self, filepath):
		f = open(filepath, 'wb');
	
		f.write(self.gameobject_pptr);
		StreamHelper.write_uint32(f, self.enabled);
		f.write(self.momoscript_pptr);
		StreamHelper.write_aligned_string(f, self.name);
		
		StreamHelper.write_aligned_string(f, self.readable_name);
		StreamHelper.write_aligned_string(f, self.readable_name_plural);
		StreamHelper.write_aligned_string(f, self.desicription);
		
		f.write(self.some_data);
		StreamHelper.write_aligned_string(f, self.yarn_node_title);
		
		f.write(self.other_data);
		
		f.close();