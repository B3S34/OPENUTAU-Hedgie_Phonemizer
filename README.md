# Hedgie's Phonemizer
	This is where I store my own homemade Phonemizer
	for specific voicebank(s) with specific style of reclist.
	
 - These Phonemizers might be update later to suited different style of
   reclist 	
 - Each Phonemizer will come with their recommended basic reclist. You still be able to add extra phonetics if its support by Phonemizer itself

 

> 

## **Hedgie's Thai VCCV Phonemizer**

This Phonemizer is use for specific reclist style of **Hedgie Thai VCCV** reclist. This provide some benefits to this reclist.

 - Dipthong oto as *VV* instead of thier own phonemes
 - Cluster oto as thier own *CC* // but sadly can't be oto as *CCV* in current time
 - Each note is 1 syllable. Vowel will not connect to neighbors note but provide as glottal stop instead.
 - Initial and Coda consonants is distinctive from another.
 
**Support Vowels Table**
|Alias|X-Sampa Equivalent |Thai Vowel Equivalent |
|--|--|--|
| a | a | อะ / อา |
| i | i | อิ / อี |
| u | M | อึ / อื |
| e | e | เอะ / เอ |
| o | o | โอะ / โอ |
| A | E | แอะ / แอ |
| E | 7 / @ | เออะ / เออ |
| O | O | เอาะ / ออ |
| U | u | อุ / อู |
|**Dipthong Table**|
|ai/ay|ai| ไอ / ใอ|
|am|am| อำ |
|aU/aw|au/aw| เอา |
|ia|ia| เอียะ / เอีย|
|ua|Ma| เอือะ / เอือ |
|Ua|ua| อัวะ / อัว |
**Support Consonant Table**
| Initial Alias | Coda Alias | Comments |
|--|--|--|
| g | ก | g in **g**ame(Non Aspirated) |
| b | บ | b in **b**ed |
| d | ด | d in **d**ice |
| n | น | n in **n**ose |
| m | ม | m in **m**ine |
| y | ย | y in **y**atch |
| w | ว | w in **w**ine |
| ng | ง | ng in si**ng** |
| ch | ฉ* | ch in **ch**ime |
| f | ฟ* | f in **f**un |
| h | ห* | h in **h**ello |
| j | จ* | j in **j**am |
| k | ค* | k in **k**ey(Aspirated) |
| l | ล* | l in **l**ow |
| p | ป* | p in ma**p**/s**p**in(Non Aspirated) |
| ph | พ* | p in **p**ie(Aspirated) |
| r | ร* | trill r in pe**rr**o |
| rr* | รร* | roll r in **r**olling |
| s | ส* | **s** in smile |
| sh | ช* | **sh** in shine |
| t | ต* | t in s**t**ar**t**/s**t**op(Non Aspirated) |
| th | ท* | t in **t**ime(Aspirated) |
| ts* | ตส* | ts in ca**ts** |
| v* | วฟ* | v in **v**i**v**id |
| z* | ซ* | z in **z**oo |
	*Doesn't occur/exist in Thai phonetics system, but exist as extra supported phonemes
**Cluster List**
Occurs in Thai phonetics : **gr kr tr thr pr phr gl kl pl phl gw kw**

Extra Cluster : **prr phrr brr frr thrr trr drr shrr krr grr vrr br fr dr shr vr bl fl sl thw dw sw tw vw by dy fy gy ky phy py my ny vy zy sp sph st sth sm sn sf spr sphr str sthr skr sprr sphrr str sthrr skrr spl sphl skl stw sthw skw sky spy sphy**

**Lyrics Input**
    Lyrics Input in "CVX" form as Consonat(C) Vowel(V) Coda(X) in all
	
	**C** can be Initial Alias, Cluster or leave it blank as glottal stop
	
	**V** can be In Vowel Alias, can be written as many as you want
	
	**X** can be Initial Alias, Phonemizer will convert to coda alias automaticly

	*Other characters that's not in these list will
