from UnityText import UnityText;
from UnityFont import UnityFont;
from StringHelper import StringHelper;
from UnityTMProFont import UnityTMProFont;
from ASH_CollectableItem import ASH_CollectableItem;
from ASH_TextMeshProUGUI import ASH_TextMeshProUGUI;
from ASH_AchievementItem import ASH_AchievementItem;
from ASH_FishSpeciese import ASH_FishSpeciese;
from ASH_UIText import ASH_UIText;

from fontTools.ttLib.ttFont import TTFont
from fontTools import subset
import os, sys, csv;

reload(sys);
sys.setdefaultencoding('utf8');

txtFiles = [
	"Text/localization.zh.yarn.txt",
	"Text/ASH_items_text.new.csv",
	"Text/ASH_menu_text.new.csv",
	"Text/ASH_achievement_text.new.csv",
	"Text/ASH_other_text.new.csv",
	"Text/ASH_fish_specise_text.new.csv",
	"Code/AuntDynamicDialogue.cs",
	"Code/CompassItem.cs",
	"Code/ControllerButtonDetector.cs",
	"Code/ControllerRemapper.cs",
	"Code/CustomControllerProfile.cs",
	"Code/Fish.cs",
	"Code/FishBuyer.cs",
	"Code/FishCollectPrompt.cs",
	"Code/FishEncyclopedia.cs",
	"Code/FishingBait.cs",
	"Code/FishItemActions.cs",
	"Code/HatItem.cs",
	"Code/Holdable.cs",
	"Code/KeyboardRemapper.cs",
	"Code/OptionsMenu.cs",
	"Code/ShowResolutionWarning.cs",
	"Code/TitleScreen.cs"
];

collectitonItems = [
	"unnamed asset-resources.assets-480.dat",
	"unnamed asset-resources.assets-481.dat",
	"unnamed asset-resources.assets-482.dat",
	"unnamed asset-resources.assets-483.dat",
	"unnamed asset-resources.assets-484.dat",
	"unnamed asset-resources.assets-485.dat",
	"unnamed asset-resources.assets-486.dat",
	"unnamed asset-resources.assets-487.dat",
	"unnamed asset-resources.assets-488.dat",
	"unnamed asset-resources.assets-489.dat",
	"unnamed asset-resources.assets-490.dat",
	"unnamed asset-resources.assets-491.dat",
	"unnamed asset-resources.assets-492.dat",
	"unnamed asset-resources.assets-493.dat",
	"unnamed asset-resources.assets-494.dat",
	"unnamed asset-resources.assets-495.dat",
	"unnamed asset-resources.assets-496.dat",
	"unnamed asset-resources.assets-497.dat",
	"unnamed asset-resources.assets-498.dat",
	"unnamed asset-resources.assets-499.dat",
	"unnamed asset-resources.assets-500.dat",
	"unnamed asset-resources.assets-501.dat",
	"unnamed asset-resources.assets-502.dat",
	"unnamed asset-resources.assets-503.dat",
	"unnamed asset-resources.assets-504.dat",
	"unnamed asset-resources.assets-505.dat",
	"unnamed asset-resources.assets-506.dat",
	"unnamed asset-resources.assets-507.dat",
	"unnamed asset-resources.assets-508.dat",
	"unnamed asset-resources.assets-509.dat"
];

menuItems = [
	"unnamed asset-level0-446.dat",
	"unnamed asset-level0-447.dat",
	"unnamed asset-level0-449.dat",
	"unnamed asset-level0-450.dat",
	"unnamed asset-level0-451.dat",
	"unnamed asset-level0-452.dat",
	"unnamed asset-sharedassets0.assets-747.dat",
	"unnamed asset-sharedassets0.assets-761.dat",
	"unnamed asset-sharedassets0.assets-782.dat",
	"unnamed asset-sharedassets0.assets-810.dat",
	"unnamed asset-sharedassets0.assets-811.dat",
	"unnamed asset-sharedassets0.assets-815.dat",
	"unnamed asset-sharedassets0.assets-820.dat",
	"unnamed asset-sharedassets0.assets-821.dat",
	"unnamed asset-sharedassets0.assets-853.dat",
	"unnamed asset-sharedassets0.assets-949.dat",
	"unnamed asset-sharedassets0.assets-950.dat",
	"unnamed asset-sharedassets0.assets-1018.dat",
	"unnamed asset-sharedassets1.assets-1175.dat",
	
	"unnamed asset-sharedassets0.assets-779.dat",
	"unnamed asset-sharedassets0.assets-783.dat",
	"unnamed asset-sharedassets0.assets-852.dat",
	"unnamed asset-sharedassets0.assets-861.dat",
	"unnamed asset-sharedassets0.assets-872.dat",
	"unnamed asset-sharedassets0.assets-880.dat",
	"unnamed asset-sharedassets0.assets-906.dat",
	"unnamed asset-sharedassets0.assets-1017.dat"
];

achievementItems = [
	"unnamed asset-resources.assets-440.dat",
	"unnamed asset-resources.assets-441.dat",
	"unnamed asset-resources.assets-442.dat",
	"unnamed asset-resources.assets-443.dat",
	"unnamed asset-resources.assets-444.dat",
	"unnamed asset-resources.assets-445.dat",
	"unnamed asset-resources.assets-446.dat",
	"unnamed asset-resources.assets-447.dat"
];

otherTexts_Steam = [
	"Steam/unnamed asset-level1-20069.dat",
	"Steam/unnamed asset-level1-20643.dat",
	"Steam/unnamed asset-level1-20825.dat",
	"Steam/unnamed asset-level1-21247.dat",
	"unnamed asset-sharedassets0.assets-905.dat",
	"unnamed asset-sharedassets0.assets-927.dat"
];

otherTexts_Epic = [
	"Epic/unnamed asset-level1-20070.dat",
	"Epic/unnamed asset-level1-20664.dat",
	"Epic/unnamed asset-level1-20846.dat",
	"Epic/unnamed asset-level1-21261.dat",
	"unnamed asset-sharedassets0.assets-905.dat",
	"unnamed asset-sharedassets0.assets-927.dat"
];

fishSpecise = [
	"unnamed asset-resources.assets-449.dat",
	"unnamed asset-resources.assets-450.dat",
	"unnamed asset-resources.assets-451.dat",
	"unnamed asset-resources.assets-452.dat",
	"unnamed asset-resources.assets-453.dat",
	"unnamed asset-resources.assets-454.dat",
	"unnamed asset-resources.assets-455.dat",
	"unnamed asset-resources.assets-456.dat",
	"unnamed asset-resources.assets-457.dat",
	"unnamed asset-resources.assets-458.dat",
	"unnamed asset-resources.assets-459.dat",
	"unnamed asset-resources.assets-460.dat",
	"unnamed asset-resources.assets-461.dat",
	"unnamed asset-resources.assets-462.dat"
];

otherTexts = [];

def get_txt():
	utxt = UnityText("OriginalFile/unnamed asset-sharedassets0.assets-107.dat");
	f = open("Text/localization.en.yarn.txt", "wb"); # suffix must be .yarn.txt
	f.write(utxt.get_txt());
	f.close();

def gen_fon():
	sh = StringHelper();
	for path in txtFiles:
		sh.add_file_text(path);
	sh.add_western();
	str = sh.get_chars();
	
	options = subset.Options()
	font = subset.load_font('C:/windows/Fonts/Cloud.ttf', options);
	subsetter = subset.Subsetter(options);
	subsetter.populate(text = str);
	subsetter.subset(font);
	subset.save_font(font, 'Font/Cloud.ttf', options);
	
	# generate unity font
	font = UnityFont("OriginalFile/unnamed asset-sharedassets0.assets-185.dat", unity_version = [2018, 0, 0, 0], version = 17);
	font.convert_to_raw("AShortHike/unnamed asset-sharedassets0.assets-185.dat", "Font/Cloud.ttf");

def gen_fon_2():
	sh = StringHelper();
	for path in txtFiles:
		sh.add_file_text(path);
	sh.add_western();

	f = open("Font/textmin.txt", "wb");
	f.write(sh.get_chars().decode("utf-8").encode("utf-8-sig"));
	f.close();
	
	# Can't add " ", make sure the path have no space.
	bmfc_filepath = "Font/Pixellari Atlas-sharedassets0.assets-69.bmfc";
	output_filepath = "Font/Pixellari Atlas-sharedassets0.assets-69.fnt";

	bmfont_tool_path = "E:\\BMFont\\bmfont.com";
	text_file_path = "\"Font\\textmin.txt\"";
	bmfc_filepath = "\"" + bmfc_filepath.replace("/", "\\") + "\"";
	output_filepath = "\"" + output_filepath.replace("/", "\\") + "\"";
	
	commandstr = " ".join((bmfont_tool_path , "-c" ,bmfc_filepath, "-o", output_filepath, "-t" ,text_file_path));
	os.system(commandstr.encode('mbcs'));
	
	fon = UnityTMProFont("OriginalFile/unnamed asset-sharedassets0.assets-710.dat", version = 17, font_version=[1, 1, 0]);
	fon.read_from_bmfont("Font/Pixellari Atlas-sharedassets0.assets-69.fnt");
	fon.faceinfo_pointSize = 16;
	fon.save_to_raw("AShortHike/unnamed asset-sharedassets0.assets-710.dat");

def get_items_text():
	f = open("Text/ASH_items_text.csv", "wb");
	csv_writer = csv.writer(f);
	
	for path in collectitonItems:
		_item = ASH_CollectableItem("OriginalFile/" + path);
		csv_writer.writerow([_item.readable_name, _item.readable_name_plural, _item.desicription, _item.yarn_node_title]);
		
	f.close();

def gen_items_text_raw():
	f = open("Text/ASH_items_text.new.csv", "rb");
	csv_reader = csv.reader(f);

	for path in collectitonItems:
		_item = ASH_CollectableItem("OriginalFile/" + path);
		row = csv_reader.next();
		_item.readable_name = row[4];
		_item.readable_name_plural = row[5];
		_item.desicription = row[6];
		_item.yarn_node_title = row[7];
		
		_item.save_to_raw("AShortHike/" + path);
		
	f.close();

def get_menu_text():
	f = open("Text/ASH_menu_text.csv", "wb");
	csv_writer = csv.writer(f);
	
	for path in menuItems:
		_item = ASH_TextMeshProUGUI("OriginalFile/" + path);
		csv_writer.writerow([_item.txt]);
		
	f.close();

def gen_menu_text_raw():
	f = open("Text/ASH_menu_text.new.csv", "rb");
	csv_reader = csv.reader(f);
	
	for path in menuItems:
		_item = ASH_TextMeshProUGUI("OriginalFile/" + path);
		row = csv_reader.next();
		_item.txt = row[1];
		_item.save_to_raw("AShortHike/" + path);
		
	f.close();

def get_achievement_text():
	f = open("Text/ASH_achievement_text.csv", "wb");
	csv_writer = csv.writer(f);
	
	for path in achievementItems:
		_item = ASH_AchievementItem("OriginalFile/" + path);
		csv_writer.writerow([_item.title, _item.desicription]);
		
	f.close();

def gen_achievement_text_raw():
	f = open("Text/ASH_achievement_text.new.csv", "rb");
	csv_reader = csv.reader(f);
	
	for path in achievementItems:
		_item = ASH_AchievementItem("OriginalFile/" + path);
		row = csv_reader.next();
		_item.title = row[2];
		_item.desicription = row[3];
		_item.save_to_raw("AShortHike/" + path);
		
	f.close();
	
def get_other_text():
	f = open("Text/ASH_other_text.csv", "wb");
	csv_writer = csv.writer(f);
	
	for path in otherTexts:
		_item = ASH_UIText("OriginalFile/" + path);
		csv_writer.writerow([_item.txt]);
		
	f.close();

def gen_other_text_raw():
	f = open("Text/ASH_other_text.new.csv", "rb");
	csv_reader = csv.reader(f);
	
	for path in otherTexts:
		_item = ASH_UIText("OriginalFile/" + path);
		row = csv_reader.next();
		_item.txt = row[1];
		_item.save_to_raw("AShortHike/" + path);
		
	f.close();

def get_fish_specise_text():
	f = open("Text/ASH_fish_specise_text.csv", "wb");
	csv_writer = csv.writer(f);
	
	for path in fishSpecise:
		_item = ASH_FishSpeciese("OriginalFile/" + path);
		csv_writer.writerow([_item.readable_name, "" ,_item.type_name, "", _item.rare_prefix_name, "", _item.journal_info]);
		
	f.close();

def gen_fish_specise_text_raw():
	f = open("Text/ASH_fish_specise_text.new.csv", "rb");
	csv_reader = csv.reader(f);

	for path in fishSpecise:
		_item = ASH_FishSpeciese("OriginalFile/" + path);
		row = csv_reader.next();
		_item.readable_name = row[1];
		_item.type_name = row[3];
		_item.rare_prefix_name = row[5];
		_item.journal_info = row[7];
		
		_item.save_to_raw("AShortHike/" + path);
		
	f.close();
	
# otherTexts = otherTexts_Steam;
otherTexts = otherTexts_Epic;
	
# get_txt();
# get_items_text();
# get_menu_text();
# get_achievement_text();
# get_other_text();
# get_fish_specise_text();

gen_fon_2();
gen_fon();
utxt = UnityText("OriginalFile/unnamed asset-sharedassets0.assets-107.dat");
utxt.load_from_txt("Text/localization.zh.yarn.txt");
utxt.save_to_raw("AShortHike/unnamed asset-sharedassets0.assets-107.dat");
gen_items_text_raw();
gen_menu_text_raw();
gen_achievement_text_raw();
gen_other_text_raw();
gen_fish_specise_text_raw();