import StreamHelper;
import sys;

reload(sys);
sys.setdefaultencoding('utf8');

class ASH_AchievementItem:
	def __init__(self, filepath):
		self.pptr_len = 12;
		self.load_form_raw(filepath);
	
	def load_form_raw(self, filepath):
		f = open(filepath, 'rb');

		self.gameobject_pptr = f.read(self.pptr_len);
		self.enabled = StreamHelper.read_uint32(f);
		self.momoscript_pptr = f.read(self.pptr_len);
		self.name = StreamHelper.read_aligned_string(f);
		
		self.sprite_pptr = f.read(self.pptr_len);
		self.locked_sprite_pptr = f.read(self.pptr_len);
		self.title = StreamHelper.read_aligned_string(f);
		self.desicription = StreamHelper.read_aligned_string(f);
		
		self.other_data = f.read(StreamHelper.get_unread_data_size(f));
		
		f.close();
		
	def save_to_raw(self, filepath):
		f = open(filepath, 'wb');
	
		f.write(self.gameobject_pptr);
		StreamHelper.write_uint32(f, self.enabled);
		f.write(self.momoscript_pptr);
		StreamHelper.write_aligned_string(f, self.name);
		
		f.write(self.sprite_pptr);
		f.write(self.locked_sprite_pptr);
		StreamHelper.write_aligned_string(f, self.title);
		StreamHelper.write_aligned_string(f, self.desicription);
		
		f.write(self.other_data);
		
		f.close();