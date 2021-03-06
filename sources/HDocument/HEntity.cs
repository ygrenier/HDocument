﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{
    /// <summary>
    /// Class for entities (aka character references) management.
    /// See http://www.w3.org/TR/html5/syntax.html#named-character-references
    /// </summary>
    public class HEntity
    {
        class EntityIndex
        {
            public Char Index { get; set; }
            public EntityIndex[] Entities { get; set; }
            public HEntity Entity { get; set; }
        }

        static IDictionary<String, HEntity> EntityNames;
        static IDictionary<Char, HEntity> EntitySingleIndex;
        static IDictionary<Char, EntityIndex> EntityIndexes;

        static HEntity()
        {
            BuildIndex();
        }

        static EntityIndex BuildIndexChain(HEntity entity, EntityIndex index, int pos)
        {
            // If not the end of chain
            if (pos < entity.Characters.Length - 1)
            {
                // Search next node in the chain
                int niPos = (index != null && index.Entities != null) ? index.Entities.TakeWhile(ei => ei.Index == entity.Characters[pos]).Count() - 1 : -1;
                EntityIndex nIndex = niPos >= 0 ? index.Entities[niPos] : null;
                // Process next node
                nIndex = BuildIndexChain(entity, nIndex, pos + 1);
                if (niPos >= 0)
                {
                    index.Entities[niPos] = nIndex;
                }
                else
                {
                    if (index == null)
                    {
                        index = new EntityIndex() {
                            Index = entity.Characters[pos]
                        };
                    }
                    if (index.Entities == null)
                    {
                        index.Entities = new EntityIndex[] { BuildIndexChain(entity, null, pos + 1) };
                    }
                    else
                    {
                        index.Entities = index.Entities.Concat(new EntityIndex[] { BuildIndexChain(entity, null, pos + 1) }).ToArray();
                    }
                    return index;
                }
            }
            return new EntityIndex() {
                Index = entity.Characters[pos],
                Entity = entity,
                Entities = null
            };
        }

        static void RegisterEntity(String name, String characters)
        {
            if (name.StartsWith("&") && name.EndsWith(";"))
                name = name.Substring(1, name.Length - 2);

            var entity = new HEntity(name, characters.ToCharArray());
            // Insert in the index name
            EntityNames[entity.Name] = entity;
            // If there is just one char, use Single Index
            if (characters.Length == 1)
            {
                EntitySingleIndex[characters[0]] = entity;
            }
            else
            {
                // Else use a chained index
                EntityIndex idx;
                if (EntityIndexes.TryGetValue(characters[0], out idx))
                {
                    EntityIndexes[characters[0]] = BuildIndexChain(entity, idx, 0);
                }
                else
                {
                    EntityIndexes[characters[0]] = BuildIndexChain(entity, null, 0);
                }
            }
        }

        static void BuildIndex()
        {
            EntityNames = new Dictionary<String, HEntity>(StringComparer.Ordinal);
            EntitySingleIndex = new Dictionary<Char, HEntity>();
            EntityIndexes = new Dictionary<Char, EntityIndex>();

            RegisterEntity("&Aacute;", "\u00C1");
            RegisterEntity("&aacute;", "\u00E1");
            RegisterEntity("&Abreve;", "\u0102");
            RegisterEntity("&abreve;", "\u0103");
            RegisterEntity("&ac;", "\u223E");
            RegisterEntity("&acd;", "\u223F");
            RegisterEntity("&acE;", "\u223E\u0333");
            RegisterEntity("&Acirc;", "\u00C2");
            RegisterEntity("&acirc;", "\u00E2");
            RegisterEntity("&acute;", "\u00B4");
            RegisterEntity("&Acy;", "\u0410");
            RegisterEntity("&acy;", "\u0430");
            RegisterEntity("&AElig;", "\u00C6");
            RegisterEntity("&aelig;", "\u00E6");
            RegisterEntity("&af;", "\u2061");
            RegisterEntity("&Afr;", "\uD835\uDD04");
            RegisterEntity("&afr;", "\uD835\uDD1E");
            RegisterEntity("&Agrave;", "\u00C0");
            RegisterEntity("&agrave;", "\u00E0");
            RegisterEntity("&alefsym;", "\u2135");
            RegisterEntity("&aleph;", "\u2135");
            RegisterEntity("&Alpha;", "\u0391");
            RegisterEntity("&alpha;", "\u03B1");
            RegisterEntity("&Amacr;", "\u0100");
            RegisterEntity("&amacr;", "\u0101");
            RegisterEntity("&amalg;", "\u2A3F");
            RegisterEntity("&AMP;", "\u0026");
            RegisterEntity("&amp;", "\u0026");
            RegisterEntity("&And;", "\u2A53");
            RegisterEntity("&and;", "\u2227");
            RegisterEntity("&andand;", "\u2A55");
            RegisterEntity("&andd;", "\u2A5C");
            RegisterEntity("&andslope;", "\u2A58");
            RegisterEntity("&andv;", "\u2A5A");
            RegisterEntity("&ang;", "\u2220");
            RegisterEntity("&ange;", "\u29A4");
            RegisterEntity("&angle;", "\u2220");
            RegisterEntity("&angmsd;", "\u2221");
            RegisterEntity("&angmsdaa;", "\u29A8");
            RegisterEntity("&angmsdab;", "\u29A9");
            RegisterEntity("&angmsdac;", "\u29AA");
            RegisterEntity("&angmsdad;", "\u29AB");
            RegisterEntity("&angmsdae;", "\u29AC");
            RegisterEntity("&angmsdaf;", "\u29AD");
            RegisterEntity("&angmsdag;", "\u29AE");
            RegisterEntity("&angmsdah;", "\u29AF");
            RegisterEntity("&angrt;", "\u221F");
            RegisterEntity("&angrtvb;", "\u22BE");
            RegisterEntity("&angrtvbd;", "\u299D");
            RegisterEntity("&angsph;", "\u2222");
            RegisterEntity("&angst;", "\u00C5");
            RegisterEntity("&angzarr;", "\u237C");
            RegisterEntity("&Aogon;", "\u0104");
            RegisterEntity("&aogon;", "\u0105");
            RegisterEntity("&Aopf;", "\uD835\uDD38");
            RegisterEntity("&aopf;", "\uD835\uDD52");
            RegisterEntity("&ap;", "\u2248");
            RegisterEntity("&apacir;", "\u2A6F");
            RegisterEntity("&apE;", "\u2A70");
            RegisterEntity("&ape;", "\u224A");
            RegisterEntity("&apid;", "\u224B");
            RegisterEntity("&apos;", "\u0027");
            RegisterEntity("&ApplyFunction;", "\u2061");
            RegisterEntity("&approx;", "\u2248");
            RegisterEntity("&approxeq;", "\u224A");
            RegisterEntity("&Aring;", "\u00C5");
            RegisterEntity("&aring;", "\u00E5");
            RegisterEntity("&Ascr;", "\uD835\uDC9C");
            RegisterEntity("&ascr;", "\uD835\uDCB6");
            RegisterEntity("&Assign;", "\u2254");
            RegisterEntity("&ast;", "\u002A");
            RegisterEntity("&asymp;", "\u2248");
            RegisterEntity("&asympeq;", "\u224D");
            RegisterEntity("&Atilde;", "\u00C3");
            RegisterEntity("&atilde;", "\u00E3");
            RegisterEntity("&Auml;", "\u00C4");
            RegisterEntity("&auml;", "\u00E4");
            RegisterEntity("&awconint;", "\u2233");
            RegisterEntity("&awint;", "\u2A11");
            RegisterEntity("&backcong;", "\u224C");
            RegisterEntity("&backepsilon;", "\u03F6");
            RegisterEntity("&backprime;", "\u2035");
            RegisterEntity("&backsim;", "\u223D");
            RegisterEntity("&backsimeq;", "\u22CD");
            RegisterEntity("&Backslash;", "\u2216");
            RegisterEntity("&Barv;", "\u2AE7");
            RegisterEntity("&barvee;", "\u22BD");
            RegisterEntity("&Barwed;", "\u2306");
            RegisterEntity("&barwed;", "\u2305");
            RegisterEntity("&barwedge;", "\u2305");
            RegisterEntity("&bbrk;", "\u23B5");
            RegisterEntity("&bbrktbrk;", "\u23B6");
            RegisterEntity("&bcong;", "\u224C");
            RegisterEntity("&Bcy;", "\u0411");
            RegisterEntity("&bcy;", "\u0431");
            RegisterEntity("&bdquo;", "\u201E");
            RegisterEntity("&becaus;", "\u2235");
            RegisterEntity("&Because;", "\u2235");
            RegisterEntity("&because;", "\u2235");
            RegisterEntity("&bemptyv;", "\u29B0");
            RegisterEntity("&bepsi;", "\u03F6");
            RegisterEntity("&bernou;", "\u212C");
            RegisterEntity("&Bernoullis;", "\u212C");
            RegisterEntity("&Beta;", "\u0392");
            RegisterEntity("&beta;", "\u03B2");
            RegisterEntity("&beth;", "\u2136");
            RegisterEntity("&between;", "\u226C");
            RegisterEntity("&Bfr;", "\uD835\uDD05");
            RegisterEntity("&bfr;", "\uD835\uDD1F");
            RegisterEntity("&bigcap;", "\u22C2");
            RegisterEntity("&bigcirc;", "\u25EF");
            RegisterEntity("&bigcup;", "\u22C3");
            RegisterEntity("&bigodot;", "\u2A00");
            RegisterEntity("&bigoplus;", "\u2A01");
            RegisterEntity("&bigotimes;", "\u2A02");
            RegisterEntity("&bigsqcup;", "\u2A06");
            RegisterEntity("&bigstar;", "\u2605");
            RegisterEntity("&bigtriangledown;", "\u25BD");
            RegisterEntity("&bigtriangleup;", "\u25B3");
            RegisterEntity("&biguplus;", "\u2A04");
            RegisterEntity("&bigvee;", "\u22C1");
            RegisterEntity("&bigwedge;", "\u22C0");
            RegisterEntity("&bkarow;", "\u290D");
            RegisterEntity("&blacklozenge;", "\u29EB");
            RegisterEntity("&blacksquare;", "\u25AA");
            RegisterEntity("&blacktriangle;", "\u25B4");
            RegisterEntity("&blacktriangledown;", "\u25BE");
            RegisterEntity("&blacktriangleleft;", "\u25C2");
            RegisterEntity("&blacktriangleright;", "\u25B8");
            RegisterEntity("&blank;", "\u2423");
            RegisterEntity("&blk12;", "\u2592");
            RegisterEntity("&blk14;", "\u2591");
            RegisterEntity("&blk34;", "\u2593");
            RegisterEntity("&block;", "\u2588");
            RegisterEntity("&bne;", "\u003D\u20E5");
            RegisterEntity("&bnequiv;", "\u2261\u20E5");
            RegisterEntity("&bNot;", "\u2AED");
            RegisterEntity("&bnot;", "\u2310");
            RegisterEntity("&Bopf;", "\uD835\uDD39");
            RegisterEntity("&bopf;", "\uD835\uDD53");
            RegisterEntity("&bot;", "\u22A5");
            RegisterEntity("&bottom;", "\u22A5");
            RegisterEntity("&bowtie;", "\u22C8");
            RegisterEntity("&boxbox;", "\u29C9");
            RegisterEntity("&boxDL;", "\u2557");
            RegisterEntity("&boxDl;", "\u2556");
            RegisterEntity("&boxdL;", "\u2555");
            RegisterEntity("&boxdl;", "\u2510");
            RegisterEntity("&boxDR;", "\u2554");
            RegisterEntity("&boxDr;", "\u2553");
            RegisterEntity("&boxdR;", "\u2552");
            RegisterEntity("&boxdr;", "\u250C");
            RegisterEntity("&boxH;", "\u2550");
            RegisterEntity("&boxh;", "\u2500");
            RegisterEntity("&boxHD;", "\u2566");
            RegisterEntity("&boxHd;", "\u2564");
            RegisterEntity("&boxhD;", "\u2565");
            RegisterEntity("&boxhd;", "\u252C");
            RegisterEntity("&boxHU;", "\u2569");
            RegisterEntity("&boxHu;", "\u2567");
            RegisterEntity("&boxhU;", "\u2568");
            RegisterEntity("&boxhu;", "\u2534");
            RegisterEntity("&boxminus;", "\u229F");
            RegisterEntity("&boxplus;", "\u229E");
            RegisterEntity("&boxtimes;", "\u22A0");
            RegisterEntity("&boxUL;", "\u255D");
            RegisterEntity("&boxUl;", "\u255C");
            RegisterEntity("&boxuL;", "\u255B");
            RegisterEntity("&boxul;", "\u2518");
            RegisterEntity("&boxUR;", "\u255A");
            RegisterEntity("&boxUr;", "\u2559");
            RegisterEntity("&boxuR;", "\u2558");
            RegisterEntity("&boxur;", "\u2514");
            RegisterEntity("&boxV;", "\u2551");
            RegisterEntity("&boxv;", "\u2502");
            RegisterEntity("&boxVH;", "\u256C");
            RegisterEntity("&boxVh;", "\u256B");
            RegisterEntity("&boxvH;", "\u256A");
            RegisterEntity("&boxvh;", "\u253C");
            RegisterEntity("&boxVL;", "\u2563");
            RegisterEntity("&boxVl;", "\u2562");
            RegisterEntity("&boxvL;", "\u2561");
            RegisterEntity("&boxvl;", "\u2524");
            RegisterEntity("&boxVR;", "\u2560");
            RegisterEntity("&boxVr;", "\u255F");
            RegisterEntity("&boxvR;", "\u255E");
            RegisterEntity("&boxvr;", "\u251C");
            RegisterEntity("&bprime;", "\u2035");
            RegisterEntity("&Breve;", "\u02D8");
            RegisterEntity("&breve;", "\u02D8");
            RegisterEntity("&brvbar;", "\u00A6");
            RegisterEntity("&Bscr;", "\u212C");
            RegisterEntity("&bscr;", "\uD835\uDCB7");
            RegisterEntity("&bsemi;", "\u204F");
            RegisterEntity("&bsim;", "\u223D");
            RegisterEntity("&bsime;", "\u22CD");
            RegisterEntity("&bsol;", "\u005C");
            RegisterEntity("&bsolb;", "\u29C5");
            RegisterEntity("&bsolhsub;", "\u27C8");
            RegisterEntity("&bull;", "\u2022");
            RegisterEntity("&bullet;", "\u2022");
            RegisterEntity("&bump;", "\u224E");
            RegisterEntity("&bumpE;", "\u2AAE");
            RegisterEntity("&bumpe;", "\u224F");
            RegisterEntity("&Bumpeq;", "\u224E");
            RegisterEntity("&bumpeq;", "\u224F");
            RegisterEntity("&Cacute;", "\u0106");
            RegisterEntity("&cacute;", "\u0107");
            RegisterEntity("&Cap;", "\u22D2");
            RegisterEntity("&cap;", "\u2229");
            RegisterEntity("&capand;", "\u2A44");
            RegisterEntity("&capbrcup;", "\u2A49");
            RegisterEntity("&capcap;", "\u2A4B");
            RegisterEntity("&capcup;", "\u2A47");
            RegisterEntity("&capdot;", "\u2A40");
            RegisterEntity("&CapitalDifferentialD;", "\u2145");
            RegisterEntity("&caps;", "\u2229\uFE00");
            RegisterEntity("&caret;", "\u2041");
            RegisterEntity("&caron;", "\u02C7");
            RegisterEntity("&Cayleys;", "\u212D");
            RegisterEntity("&ccaps;", "\u2A4D");
            RegisterEntity("&Ccaron;", "\u010C");
            RegisterEntity("&ccaron;", "\u010D");
            RegisterEntity("&Ccedil;", "\u00C7");
            RegisterEntity("&ccedil;", "\u00E7");
            RegisterEntity("&Ccirc;", "\u0108");
            RegisterEntity("&ccirc;", "\u0109");
            RegisterEntity("&Cconint;", "\u2230");
            RegisterEntity("&ccups;", "\u2A4C");
            RegisterEntity("&ccupssm;", "\u2A50");
            RegisterEntity("&Cdot;", "\u010A");
            RegisterEntity("&cdot;", "\u010B");
            RegisterEntity("&cedil;", "\u00B8");
            RegisterEntity("&Cedilla;", "\u00B8");
            RegisterEntity("&cemptyv;", "\u29B2");
            RegisterEntity("&cent;", "\u00A2");
            RegisterEntity("&CenterDot;", "\u00B7");
            RegisterEntity("&centerdot;", "\u00B7");
            RegisterEntity("&Cfr;", "\u212D");
            RegisterEntity("&cfr;", "\uD835\uDD20");
            RegisterEntity("&CHcy;", "\u0427");
            RegisterEntity("&chcy;", "\u0447");
            RegisterEntity("&check;", "\u2713");
            RegisterEntity("&checkmark;", "\u2713");
            RegisterEntity("&Chi;", "\u03A7");
            RegisterEntity("&chi;", "\u03C7");
            RegisterEntity("&cir;", "\u25CB");
            RegisterEntity("&circ;", "\u02C6");
            RegisterEntity("&circeq;", "\u2257");
            RegisterEntity("&circlearrowleft;", "\u21BA");
            RegisterEntity("&circlearrowright;", "\u21BB");
            RegisterEntity("&circledast;", "\u229B");
            RegisterEntity("&circledcirc;", "\u229A");
            RegisterEntity("&circleddash;", "\u229D");
            RegisterEntity("&CircleDot;", "\u2299");
            RegisterEntity("&circledR;", "\u00AE");
            RegisterEntity("&circledS;", "\u24C8");
            RegisterEntity("&CircleMinus;", "\u2296");
            RegisterEntity("&CirclePlus;", "\u2295");
            RegisterEntity("&CircleTimes;", "\u2297");
            RegisterEntity("&cirE;", "\u29C3");
            RegisterEntity("&cire;", "\u2257");
            RegisterEntity("&cirfnint;", "\u2A10");
            RegisterEntity("&cirmid;", "\u2AEF");
            RegisterEntity("&cirscir;", "\u29C2");
            RegisterEntity("&ClockwiseContourIntegral;", "\u2232");
            RegisterEntity("&CloseCurlyDoubleQuote;", "\u201D");
            RegisterEntity("&CloseCurlyQuote;", "\u2019");
            RegisterEntity("&clubs;", "\u2663");
            RegisterEntity("&clubsuit;", "\u2663");
            RegisterEntity("&Colon;", "\u2237");
            RegisterEntity("&colon;", "\u003A");
            RegisterEntity("&Colone;", "\u2A74");
            RegisterEntity("&colone;", "\u2254");
            RegisterEntity("&coloneq;", "\u2254");
            RegisterEntity("&comma;", "\u002C");
            RegisterEntity("&commat;", "\u0040");
            RegisterEntity("&comp;", "\u2201");
            RegisterEntity("&compfn;", "\u2218");
            RegisterEntity("&complement;", "\u2201");
            RegisterEntity("&complexes;", "\u2102");
            RegisterEntity("&cong;", "\u2245");
            RegisterEntity("&congdot;", "\u2A6D");
            RegisterEntity("&Congruent;", "\u2261");
            RegisterEntity("&Conint;", "\u222F");
            RegisterEntity("&conint;", "\u222E");
            RegisterEntity("&ContourIntegral;", "\u222E");
            RegisterEntity("&Copf;", "\u2102");
            RegisterEntity("&copf;", "\uD835\uDD54");
            RegisterEntity("&coprod;", "\u2210");
            RegisterEntity("&Coproduct;", "\u2210");
            RegisterEntity("&COPY;", "\u00A9");
            RegisterEntity("&copy;", "\u00A9");
            RegisterEntity("&copysr;", "\u2117");
            RegisterEntity("&CounterClockwiseContourIntegral;", "\u2233");
            RegisterEntity("&crarr;", "\u21B5");
            RegisterEntity("&Cross;", "\u2A2F");
            RegisterEntity("&cross;", "\u2717");
            RegisterEntity("&Cscr;", "\uD835\uDC9E");
            RegisterEntity("&cscr;", "\uD835\uDCB8");
            RegisterEntity("&csub;", "\u2ACF");
            RegisterEntity("&csube;", "\u2AD1");
            RegisterEntity("&csup;", "\u2AD0");
            RegisterEntity("&csupe;", "\u2AD2");
            RegisterEntity("&ctdot;", "\u22EF");
            RegisterEntity("&cudarrl;", "\u2938");
            RegisterEntity("&cudarrr;", "\u2935");
            RegisterEntity("&cuepr;", "\u22DE");
            RegisterEntity("&cuesc;", "\u22DF");
            RegisterEntity("&cularr;", "\u21B6");
            RegisterEntity("&cularrp;", "\u293D");
            RegisterEntity("&Cup;", "\u22D3");
            RegisterEntity("&cup;", "\u222A");
            RegisterEntity("&cupbrcap;", "\u2A48");
            RegisterEntity("&CupCap;", "\u224D");
            RegisterEntity("&cupcap;", "\u2A46");
            RegisterEntity("&cupcup;", "\u2A4A");
            RegisterEntity("&cupdot;", "\u228D");
            RegisterEntity("&cupor;", "\u2A45");
            RegisterEntity("&cups;", "\u222A\uFE00");
            RegisterEntity("&curarr;", "\u21B7");
            RegisterEntity("&curarrm;", "\u293C");
            RegisterEntity("&curlyeqprec;", "\u22DE");
            RegisterEntity("&curlyeqsucc;", "\u22DF");
            RegisterEntity("&curlyvee;", "\u22CE");
            RegisterEntity("&curlywedge;", "\u22CF");
            RegisterEntity("&curren;", "\u00A4");
            RegisterEntity("&curvearrowleft;", "\u21B6");
            RegisterEntity("&curvearrowright;", "\u21B7");
            RegisterEntity("&cuvee;", "\u22CE");
            RegisterEntity("&cuwed;", "\u22CF");
            RegisterEntity("&cwconint;", "\u2232");
            RegisterEntity("&cwint;", "\u2231");
            RegisterEntity("&cylcty;", "\u232D");
            RegisterEntity("&Dagger;", "\u2021");
            RegisterEntity("&dagger;", "\u2020");
            RegisterEntity("&daleth;", "\u2138");
            RegisterEntity("&Darr;", "\u21A1");
            RegisterEntity("&dArr;", "\u21D3");
            RegisterEntity("&darr;", "\u2193");
            RegisterEntity("&dash;", "\u2010");
            RegisterEntity("&Dashv;", "\u2AE4");
            RegisterEntity("&dashv;", "\u22A3");
            RegisterEntity("&dbkarow;", "\u290F");
            RegisterEntity("&dblac;", "\u02DD");
            RegisterEntity("&Dcaron;", "\u010E");
            RegisterEntity("&dcaron;", "\u010F");
            RegisterEntity("&Dcy;", "\u0414");
            RegisterEntity("&dcy;", "\u0434");
            RegisterEntity("&DD;", "\u2145");
            RegisterEntity("&dd;", "\u2146");
            RegisterEntity("&ddagger;", "\u2021");
            RegisterEntity("&ddarr;", "\u21CA");
            RegisterEntity("&DDotrahd;", "\u2911");
            RegisterEntity("&ddotseq;", "\u2A77");
            RegisterEntity("&deg;", "\u00B0");
            RegisterEntity("&Del;", "\u2207");
            RegisterEntity("&Delta;", "\u0394");
            RegisterEntity("&delta;", "\u03B4");
            RegisterEntity("&demptyv;", "\u29B1");
            RegisterEntity("&dfisht;", "\u297F");
            RegisterEntity("&Dfr;", "\uD835\uDD07");
            RegisterEntity("&dfr;", "\uD835\uDD21");
            RegisterEntity("&dHar;", "\u2965");
            RegisterEntity("&dharl;", "\u21C3");
            RegisterEntity("&dharr;", "\u21C2");
            RegisterEntity("&DiacriticalAcute;", "\u00B4");
            RegisterEntity("&DiacriticalDot;", "\u02D9");
            RegisterEntity("&DiacriticalDoubleAcute;", "\u02DD");
            RegisterEntity("&DiacriticalGrave;", "\u0060");
            RegisterEntity("&DiacriticalTilde;", "\u02DC");
            RegisterEntity("&diam;", "\u22C4");
            RegisterEntity("&Diamond;", "\u22C4");
            RegisterEntity("&diamond;", "\u22C4");
            RegisterEntity("&diamondsuit;", "\u2666");
            RegisterEntity("&diams;", "\u2666");
            RegisterEntity("&die;", "\u00A8");
            RegisterEntity("&DifferentialD;", "\u2146");
            RegisterEntity("&digamma;", "\u03DD");
            RegisterEntity("&disin;", "\u22F2");
            RegisterEntity("&div;", "\u00F7");
            RegisterEntity("&divide;", "\u00F7");
            RegisterEntity("&divideontimes;", "\u22C7");
            RegisterEntity("&divonx;", "\u22C7");
            RegisterEntity("&DJcy;", "\u0402");
            RegisterEntity("&djcy;", "\u0452");
            RegisterEntity("&dlcorn;", "\u231E");
            RegisterEntity("&dlcrop;", "\u230D");
            RegisterEntity("&dollar;", "\u0024");
            RegisterEntity("&Dopf;", "\uD835\uDD3B");
            RegisterEntity("&dopf;", "\uD835\uDD55");
            RegisterEntity("&Dot;", "\u00A8");
            RegisterEntity("&dot;", "\u02D9");
            RegisterEntity("&DotDot;", "\u20DC");
            RegisterEntity("&doteq;", "\u2250");
            RegisterEntity("&doteqdot;", "\u2251");
            RegisterEntity("&DotEqual;", "\u2250");
            RegisterEntity("&dotminus;", "\u2238");
            RegisterEntity("&dotplus;", "\u2214");
            RegisterEntity("&dotsquare;", "\u22A1");
            RegisterEntity("&doublebarwedge;", "\u2306");
            RegisterEntity("&DoubleContourIntegral;", "\u222F");
            RegisterEntity("&DoubleDot;", "\u00A8");
            RegisterEntity("&DoubleDownArrow;", "\u21D3");
            RegisterEntity("&DoubleLeftArrow;", "\u21D0");
            RegisterEntity("&DoubleLeftRightArrow;", "\u21D4");
            RegisterEntity("&DoubleLeftTee;", "\u2AE4");
            RegisterEntity("&DoubleLongLeftArrow;", "\u27F8");
            RegisterEntity("&DoubleLongLeftRightArrow;", "\u27FA");
            RegisterEntity("&DoubleLongRightArrow;", "\u27F9");
            RegisterEntity("&DoubleRightArrow;", "\u21D2");
            RegisterEntity("&DoubleRightTee;", "\u22A8");
            RegisterEntity("&DoubleUpArrow;", "\u21D1");
            RegisterEntity("&DoubleUpDownArrow;", "\u21D5");
            RegisterEntity("&DoubleVerticalBar;", "\u2225");
            RegisterEntity("&DownArrow;", "\u2193");
            RegisterEntity("&Downarrow;", "\u21D3");
            RegisterEntity("&downarrow;", "\u2193");
            RegisterEntity("&DownArrowBar;", "\u2913");
            RegisterEntity("&DownArrowUpArrow;", "\u21F5");
            RegisterEntity("&DownBreve;", "\u0311");
            RegisterEntity("&downdownarrows;", "\u21CA");
            RegisterEntity("&downharpoonleft;", "\u21C3");
            RegisterEntity("&downharpoonright;", "\u21C2");
            RegisterEntity("&DownLeftRightVector;", "\u2950");
            RegisterEntity("&DownLeftTeeVector;", "\u295E");
            RegisterEntity("&DownLeftVector;", "\u21BD");
            RegisterEntity("&DownLeftVectorBar;", "\u2956");
            RegisterEntity("&DownRightTeeVector;", "\u295F");
            RegisterEntity("&DownRightVector;", "\u21C1");
            RegisterEntity("&DownRightVectorBar;", "\u2957");
            RegisterEntity("&DownTee;", "\u22A4");
            RegisterEntity("&DownTeeArrow;", "\u21A7");
            RegisterEntity("&drbkarow;", "\u2910");
            RegisterEntity("&drcorn;", "\u231F");
            RegisterEntity("&drcrop;", "\u230C");
            RegisterEntity("&Dscr;", "\uD835\uDC9F");
            RegisterEntity("&dscr;", "\uD835\uDCB9");
            RegisterEntity("&DScy;", "\u0405");
            RegisterEntity("&dscy;", "\u0455");
            RegisterEntity("&dsol;", "\u29F6");
            RegisterEntity("&Dstrok;", "\u0110");
            RegisterEntity("&dstrok;", "\u0111");
            RegisterEntity("&dtdot;", "\u22F1");
            RegisterEntity("&dtri;", "\u25BF");
            RegisterEntity("&dtrif;", "\u25BE");
            RegisterEntity("&duarr;", "\u21F5");
            RegisterEntity("&duhar;", "\u296F");
            RegisterEntity("&dwangle;", "\u29A6");
            RegisterEntity("&DZcy;", "\u040F");
            RegisterEntity("&dzcy;", "\u045F");
            RegisterEntity("&dzigrarr;", "\u27FF");
            RegisterEntity("&Eacute;", "\u00C9");
            RegisterEntity("&eacute;", "\u00E9");
            RegisterEntity("&easter;", "\u2A6E");
            RegisterEntity("&Ecaron;", "\u011A");
            RegisterEntity("&ecaron;", "\u011B");
            RegisterEntity("&ecir;", "\u2256");
            RegisterEntity("&Ecirc;", "\u00CA");
            RegisterEntity("&ecirc;", "\u00EA");
            RegisterEntity("&ecolon;", "\u2255");
            RegisterEntity("&Ecy;", "\u042D");
            RegisterEntity("&ecy;", "\u044D");
            RegisterEntity("&eDDot;", "\u2A77");
            RegisterEntity("&Edot;", "\u0116");
            RegisterEntity("&eDot;", "\u2251");
            RegisterEntity("&edot;", "\u0117");
            RegisterEntity("&ee;", "\u2147");
            RegisterEntity("&efDot;", "\u2252");
            RegisterEntity("&Efr;", "\uD835\uDD08");
            RegisterEntity("&efr;", "\uD835\uDD22");
            RegisterEntity("&eg;", "\u2A9A");
            RegisterEntity("&Egrave;", "\u00C8");
            RegisterEntity("&egrave;", "\u00E8");
            RegisterEntity("&egs;", "\u2A96");
            RegisterEntity("&egsdot;", "\u2A98");
            RegisterEntity("&el;", "\u2A99");
            RegisterEntity("&Element;", "\u2208");
            RegisterEntity("&elinters;", "\u23E7");
            RegisterEntity("&ell;", "\u2113");
            RegisterEntity("&els;", "\u2A95");
            RegisterEntity("&elsdot;", "\u2A97");
            RegisterEntity("&Emacr;", "\u0112");
            RegisterEntity("&emacr;", "\u0113");
            RegisterEntity("&empty;", "\u2205");
            RegisterEntity("&emptyset;", "\u2205");
            RegisterEntity("&EmptySmallSquare;", "\u25FB");
            RegisterEntity("&emptyv;", "\u2205");
            RegisterEntity("&EmptyVerySmallSquare;", "\u25AB");
            RegisterEntity("&emsp;", "\u2003");
            RegisterEntity("&emsp13;", "\u2004");
            RegisterEntity("&emsp14;", "\u2005");
            RegisterEntity("&ENG;", "\u014A");
            RegisterEntity("&eng;", "\u014B");
            RegisterEntity("&ensp;", "\u2002");
            RegisterEntity("&Eogon;", "\u0118");
            RegisterEntity("&eogon;", "\u0119");
            RegisterEntity("&Eopf;", "\uD835\uDD3C");
            RegisterEntity("&eopf;", "\uD835\uDD56");
            RegisterEntity("&epar;", "\u22D5");
            RegisterEntity("&eparsl;", "\u29E3");
            RegisterEntity("&eplus;", "\u2A71");
            RegisterEntity("&epsi;", "\u03B5");
            RegisterEntity("&Epsilon;", "\u0395");
            RegisterEntity("&epsilon;", "\u03B5");
            RegisterEntity("&epsiv;", "\u03F5");
            RegisterEntity("&eqcirc;", "\u2256");
            RegisterEntity("&eqcolon;", "\u2255");
            RegisterEntity("&eqsim;", "\u2242");
            RegisterEntity("&eqslantgtr;", "\u2A96");
            RegisterEntity("&eqslantless;", "\u2A95");
            RegisterEntity("&Equal;", "\u2A75");
            RegisterEntity("&equals;", "\u003D");
            RegisterEntity("&EqualTilde;", "\u2242");
            RegisterEntity("&equest;", "\u225F");
            RegisterEntity("&Equilibrium;", "\u21CC");
            RegisterEntity("&equiv;", "\u2261");
            RegisterEntity("&equivDD;", "\u2A78");
            RegisterEntity("&eqvparsl;", "\u29E5");
            RegisterEntity("&erarr;", "\u2971");
            RegisterEntity("&erDot;", "\u2253");
            RegisterEntity("&Escr;", "\u2130");
            RegisterEntity("&escr;", "\u212F");
            RegisterEntity("&esdot;", "\u2250");
            RegisterEntity("&Esim;", "\u2A73");
            RegisterEntity("&esim;", "\u2242");
            RegisterEntity("&Eta;", "\u0397");
            RegisterEntity("&eta;", "\u03B7");
            RegisterEntity("&ETH;", "\u00D0");
            RegisterEntity("&eth;", "\u00F0");
            RegisterEntity("&Euml;", "\u00CB");
            RegisterEntity("&euml;", "\u00EB");
            RegisterEntity("&euro;", "\u20AC");
            RegisterEntity("&excl;", "\u0021");
            RegisterEntity("&exist;", "\u2203");
            RegisterEntity("&Exists;", "\u2203");
            RegisterEntity("&expectation;", "\u2130");
            RegisterEntity("&ExponentialE;", "\u2147");
            RegisterEntity("&exponentiale;", "\u2147");
            RegisterEntity("&fallingdotseq;", "\u2252");
            RegisterEntity("&Fcy;", "\u0424");
            RegisterEntity("&fcy;", "\u0444");
            RegisterEntity("&female;", "\u2640");
            RegisterEntity("&ffilig;", "\uFB03");
            RegisterEntity("&fflig;", "\uFB00");
            RegisterEntity("&ffllig;", "\uFB04");
            RegisterEntity("&Ffr;", "\uD835\uDD09");
            RegisterEntity("&ffr;", "\uD835\uDD23");
            RegisterEntity("&filig;", "\uFB01");
            RegisterEntity("&FilledSmallSquare;", "\u25FC");
            RegisterEntity("&FilledVerySmallSquare;", "\u25AA");
            RegisterEntity("&fjlig;", "\u0066\u006A");
            RegisterEntity("&flat;", "\u266D");
            RegisterEntity("&fllig;", "\uFB02");
            RegisterEntity("&fltns;", "\u25B1");
            RegisterEntity("&fnof;", "\u0192");
            RegisterEntity("&Fopf;", "\uD835\uDD3D");
            RegisterEntity("&fopf;", "\uD835\uDD57");
            RegisterEntity("&ForAll;", "\u2200");
            RegisterEntity("&forall;", "\u2200");
            RegisterEntity("&fork;", "\u22D4");
            RegisterEntity("&forkv;", "\u2AD9");
            RegisterEntity("&Fouriertrf;", "\u2131");
            RegisterEntity("&fpartint;", "\u2A0D");
            RegisterEntity("&frac12;", "\u00BD");
            RegisterEntity("&frac13;", "\u2153");
            RegisterEntity("&frac14;", "\u00BC");
            RegisterEntity("&frac15;", "\u2155");
            RegisterEntity("&frac16;", "\u2159");
            RegisterEntity("&frac18;", "\u215B");
            RegisterEntity("&frac23;", "\u2154");
            RegisterEntity("&frac25;", "\u2156");
            RegisterEntity("&frac34;", "\u00BE");
            RegisterEntity("&frac35;", "\u2157");
            RegisterEntity("&frac38;", "\u215C");
            RegisterEntity("&frac45;", "\u2158");
            RegisterEntity("&frac56;", "\u215A");
            RegisterEntity("&frac58;", "\u215D");
            RegisterEntity("&frac78;", "\u215E");
            RegisterEntity("&frasl;", "\u2044");
            RegisterEntity("&frown;", "\u2322");
            RegisterEntity("&Fscr;", "\u2131");
            RegisterEntity("&fscr;", "\uD835\uDCBB");
            RegisterEntity("&gacute;", "\u01F5");
            RegisterEntity("&Gamma;", "\u0393");
            RegisterEntity("&gamma;", "\u03B3");
            RegisterEntity("&Gammad;", "\u03DC");
            RegisterEntity("&gammad;", "\u03DD");
            RegisterEntity("&gap;", "\u2A86");
            RegisterEntity("&Gbreve;", "\u011E");
            RegisterEntity("&gbreve;", "\u011F");
            RegisterEntity("&Gcedil;", "\u0122");
            RegisterEntity("&Gcirc;", "\u011C");
            RegisterEntity("&gcirc;", "\u011D");
            RegisterEntity("&Gcy;", "\u0413");
            RegisterEntity("&gcy;", "\u0433");
            RegisterEntity("&Gdot;", "\u0120");
            RegisterEntity("&gdot;", "\u0121");
            RegisterEntity("&gE;", "\u2267");
            RegisterEntity("&ge;", "\u2265");
            RegisterEntity("&gEl;", "\u2A8C");
            RegisterEntity("&gel;", "\u22DB");
            RegisterEntity("&geq;", "\u2265");
            RegisterEntity("&geqq;", "\u2267");
            RegisterEntity("&geqslant;", "\u2A7E");
            RegisterEntity("&ges;", "\u2A7E");
            RegisterEntity("&gescc;", "\u2AA9");
            RegisterEntity("&gesdot;", "\u2A80");
            RegisterEntity("&gesdoto;", "\u2A82");
            RegisterEntity("&gesdotol;", "\u2A84");
            RegisterEntity("&gesl;", "\u22DB\uFE00");
            RegisterEntity("&gesles;", "\u2A94");
            RegisterEntity("&Gfr;", "\uD835\uDD0A");
            RegisterEntity("&gfr;", "\uD835\uDD24");
            RegisterEntity("&Gg;", "\u22D9");
            RegisterEntity("&gg;", "\u226B");
            RegisterEntity("&ggg;", "\u22D9");
            RegisterEntity("&gimel;", "\u2137");
            RegisterEntity("&GJcy;", "\u0403");
            RegisterEntity("&gjcy;", "\u0453");
            RegisterEntity("&gl;", "\u2277");
            RegisterEntity("&gla;", "\u2AA5");
            RegisterEntity("&glE;", "\u2A92");
            RegisterEntity("&glj;", "\u2AA4");
            RegisterEntity("&gnap;", "\u2A8A");
            RegisterEntity("&gnapprox;", "\u2A8A");
            RegisterEntity("&gnE;", "\u2269");
            RegisterEntity("&gne;", "\u2A88");
            RegisterEntity("&gneq;", "\u2A88");
            RegisterEntity("&gneqq;", "\u2269");
            RegisterEntity("&gnsim;", "\u22E7");
            RegisterEntity("&Gopf;", "\uD835\uDD3E");
            RegisterEntity("&gopf;", "\uD835\uDD58");
            RegisterEntity("&grave;", "\u0060");
            RegisterEntity("&GreaterEqual;", "\u2265");
            RegisterEntity("&GreaterEqualLess;", "\u22DB");
            RegisterEntity("&GreaterFullEqual;", "\u2267");
            RegisterEntity("&GreaterGreater;", "\u2AA2");
            RegisterEntity("&GreaterLess;", "\u2277");
            RegisterEntity("&GreaterSlantEqual;", "\u2A7E");
            RegisterEntity("&GreaterTilde;", "\u2273");
            RegisterEntity("&Gscr;", "\uD835\uDCA2");
            RegisterEntity("&gscr;", "\u210A");
            RegisterEntity("&gsim;", "\u2273");
            RegisterEntity("&gsime;", "\u2A8E");
            RegisterEntity("&gsiml;", "\u2A90");
            RegisterEntity("&GT;", "\u003E");
            RegisterEntity("&Gt;", "\u226B");
            RegisterEntity("&gt;", "\u003E");
            RegisterEntity("&gtcc;", "\u2AA7");
            RegisterEntity("&gtcir;", "\u2A7A");
            RegisterEntity("&gtdot;", "\u22D7");
            RegisterEntity("&gtlPar;", "\u2995");
            RegisterEntity("&gtquest;", "\u2A7C");
            RegisterEntity("&gtrapprox;", "\u2A86");
            RegisterEntity("&gtrarr;", "\u2978");
            RegisterEntity("&gtrdot;", "\u22D7");
            RegisterEntity("&gtreqless;", "\u22DB");
            RegisterEntity("&gtreqqless;", "\u2A8C");
            RegisterEntity("&gtrless;", "\u2277");
            RegisterEntity("&gtrsim;", "\u2273");
            RegisterEntity("&gvertneqq;", "\u2269\uFE00");
            RegisterEntity("&gvnE;", "\u2269\uFE00");
            RegisterEntity("&Hacek;", "\u02C7");
            RegisterEntity("&hairsp;", "\u200A");
            RegisterEntity("&half;", "\u00BD");
            RegisterEntity("&hamilt;", "\u210B");
            RegisterEntity("&HARDcy;", "\u042A");
            RegisterEntity("&hardcy;", "\u044A");
            RegisterEntity("&hArr;", "\u21D4");
            RegisterEntity("&harr;", "\u2194");
            RegisterEntity("&harrcir;", "\u2948");
            RegisterEntity("&harrw;", "\u21AD");
            RegisterEntity("&Hat;", "\u005E");
            RegisterEntity("&hbar;", "\u210F");
            RegisterEntity("&Hcirc;", "\u0124");
            RegisterEntity("&hcirc;", "\u0125");
            RegisterEntity("&hearts;", "\u2665");
            RegisterEntity("&heartsuit;", "\u2665");
            RegisterEntity("&hellip;", "\u2026");
            RegisterEntity("&hercon;", "\u22B9");
            RegisterEntity("&Hfr;", "\u210C");
            RegisterEntity("&hfr;", "\uD835\uDD25");
            RegisterEntity("&HilbertSpace;", "\u210B");
            RegisterEntity("&hksearow;", "\u2925");
            RegisterEntity("&hkswarow;", "\u2926");
            RegisterEntity("&hoarr;", "\u21FF");
            RegisterEntity("&homtht;", "\u223B");
            RegisterEntity("&hookleftarrow;", "\u21A9");
            RegisterEntity("&hookrightarrow;", "\u21AA");
            RegisterEntity("&Hopf;", "\u210D");
            RegisterEntity("&hopf;", "\uD835\uDD59");
            RegisterEntity("&horbar;", "\u2015");
            RegisterEntity("&HorizontalLine;", "\u2500");
            RegisterEntity("&Hscr;", "\u210B");
            RegisterEntity("&hscr;", "\uD835\uDCBD");
            RegisterEntity("&hslash;", "\u210F");
            RegisterEntity("&Hstrok;", "\u0126");
            RegisterEntity("&hstrok;", "\u0127");
            RegisterEntity("&HumpDownHump;", "\u224E");
            RegisterEntity("&HumpEqual;", "\u224F");
            RegisterEntity("&hybull;", "\u2043");
            RegisterEntity("&hyphen;", "\u2010");
            RegisterEntity("&Iacute;", "\u00CD");
            RegisterEntity("&iacute;", "\u00ED");
            RegisterEntity("&ic;", "\u2063");
            RegisterEntity("&Icirc;", "\u00CE");
            RegisterEntity("&icirc;", "\u00EE");
            RegisterEntity("&Icy;", "\u0418");
            RegisterEntity("&icy;", "\u0438");
            RegisterEntity("&Idot;", "\u0130");
            RegisterEntity("&IEcy;", "\u0415");
            RegisterEntity("&iecy;", "\u0435");
            RegisterEntity("&iexcl;", "\u00A1");
            RegisterEntity("&iff;", "\u21D4");
            RegisterEntity("&Ifr;", "\u2111");
            RegisterEntity("&ifr;", "\uD835\uDD26");
            RegisterEntity("&Igrave;", "\u00CC");
            RegisterEntity("&igrave;", "\u00EC");
            RegisterEntity("&ii;", "\u2148");
            RegisterEntity("&iiiint;", "\u2A0C");
            RegisterEntity("&iiint;", "\u222D");
            RegisterEntity("&iinfin;", "\u29DC");
            RegisterEntity("&iiota;", "\u2129");
            RegisterEntity("&IJlig;", "\u0132");
            RegisterEntity("&ijlig;", "\u0133");
            RegisterEntity("&Im;", "\u2111");
            RegisterEntity("&Imacr;", "\u012A");
            RegisterEntity("&imacr;", "\u012B");
            RegisterEntity("&image;", "\u2111");
            RegisterEntity("&ImaginaryI;", "\u2148");
            RegisterEntity("&imagline;", "\u2110");
            RegisterEntity("&imagpart;", "\u2111");
            RegisterEntity("&imath;", "\u0131");
            RegisterEntity("&imof;", "\u22B7");
            RegisterEntity("&imped;", "\u01B5");
            RegisterEntity("&Implies;", "\u21D2");
            RegisterEntity("&in;", "\u2208");
            RegisterEntity("&incare;", "\u2105");
            RegisterEntity("&infin;", "\u221E");
            RegisterEntity("&infintie;", "\u29DD");
            RegisterEntity("&inodot;", "\u0131");
            RegisterEntity("&Int;", "\u222C");
            RegisterEntity("&int;", "\u222B");
            RegisterEntity("&intcal;", "\u22BA");
            RegisterEntity("&integers;", "\u2124");
            RegisterEntity("&Integral;", "\u222B");
            RegisterEntity("&intercal;", "\u22BA");
            RegisterEntity("&Intersection;", "\u22C2");
            RegisterEntity("&intlarhk;", "\u2A17");
            RegisterEntity("&intprod;", "\u2A3C");
            RegisterEntity("&InvisibleComma;", "\u2063");
            RegisterEntity("&InvisibleTimes;", "\u2062");
            RegisterEntity("&IOcy;", "\u0401");
            RegisterEntity("&iocy;", "\u0451");
            RegisterEntity("&Iogon;", "\u012E");
            RegisterEntity("&iogon;", "\u012F");
            RegisterEntity("&Iopf;", "\uD835\uDD40");
            RegisterEntity("&iopf;", "\uD835\uDD5A");
            RegisterEntity("&Iota;", "\u0399");
            RegisterEntity("&iota;", "\u03B9");
            RegisterEntity("&iprod;", "\u2A3C");
            RegisterEntity("&iquest;", "\u00BF");
            RegisterEntity("&Iscr;", "\u2110");
            RegisterEntity("&iscr;", "\uD835\uDCBE");
            RegisterEntity("&isin;", "\u2208");
            RegisterEntity("&isindot;", "\u22F5");
            RegisterEntity("&isinE;", "\u22F9");
            RegisterEntity("&isins;", "\u22F4");
            RegisterEntity("&isinsv;", "\u22F3");
            RegisterEntity("&isinv;", "\u2208");
            RegisterEntity("&it;", "\u2062");
            RegisterEntity("&Itilde;", "\u0128");
            RegisterEntity("&itilde;", "\u0129");
            RegisterEntity("&Iukcy;", "\u0406");
            RegisterEntity("&iukcy;", "\u0456");
            RegisterEntity("&Iuml;", "\u00CF");
            RegisterEntity("&iuml;", "\u00EF");
            RegisterEntity("&Jcirc;", "\u0134");
            RegisterEntity("&jcirc;", "\u0135");
            RegisterEntity("&Jcy;", "\u0419");
            RegisterEntity("&jcy;", "\u0439");
            RegisterEntity("&Jfr;", "\uD835\uDD0D");
            RegisterEntity("&jfr;", "\uD835\uDD27");
            RegisterEntity("&jmath;", "\u0237");
            RegisterEntity("&Jopf;", "\uD835\uDD41");
            RegisterEntity("&jopf;", "\uD835\uDD5B");
            RegisterEntity("&Jscr;", "\uD835\uDCA5");
            RegisterEntity("&jscr;", "\uD835\uDCBF");
            RegisterEntity("&Jsercy;", "\u0408");
            RegisterEntity("&jsercy;", "\u0458");
            RegisterEntity("&Jukcy;", "\u0404");
            RegisterEntity("&jukcy;", "\u0454");
            RegisterEntity("&Kappa;", "\u039A");
            RegisterEntity("&kappa;", "\u03BA");
            RegisterEntity("&kappav;", "\u03F0");
            RegisterEntity("&Kcedil;", "\u0136");
            RegisterEntity("&kcedil;", "\u0137");
            RegisterEntity("&Kcy;", "\u041A");
            RegisterEntity("&kcy;", "\u043A");
            RegisterEntity("&Kfr;", "\uD835\uDD0E");
            RegisterEntity("&kfr;", "\uD835\uDD28");
            RegisterEntity("&kgreen;", "\u0138");
            RegisterEntity("&KHcy;", "\u0425");
            RegisterEntity("&khcy;", "\u0445");
            RegisterEntity("&KJcy;", "\u040C");
            RegisterEntity("&kjcy;", "\u045C");
            RegisterEntity("&Kopf;", "\uD835\uDD42");
            RegisterEntity("&kopf;", "\uD835\uDD5C");
            RegisterEntity("&Kscr;", "\uD835\uDCA6");
            RegisterEntity("&kscr;", "\uD835\uDCC0");
            RegisterEntity("&lAarr;", "\u21DA");
            RegisterEntity("&Lacute;", "\u0139");
            RegisterEntity("&lacute;", "\u013A");
            RegisterEntity("&laemptyv;", "\u29B4");
            RegisterEntity("&lagran;", "\u2112");
            RegisterEntity("&Lambda;", "\u039B");
            RegisterEntity("&lambda;", "\u03BB");
            RegisterEntity("&Lang;", "\u27EA");
            RegisterEntity("&lang;", "\u27E8");
            RegisterEntity("&langd;", "\u2991");
            RegisterEntity("&langle;", "\u27E8");
            RegisterEntity("&lap;", "\u2A85");
            RegisterEntity("&Laplacetrf;", "\u2112");
            RegisterEntity("&laquo;", "\u00AB");
            RegisterEntity("&Larr;", "\u219E");
            RegisterEntity("&lArr;", "\u21D0");
            RegisterEntity("&larr;", "\u2190");
            RegisterEntity("&larrb;", "\u21E4");
            RegisterEntity("&larrbfs;", "\u291F");
            RegisterEntity("&larrfs;", "\u291D");
            RegisterEntity("&larrhk;", "\u21A9");
            RegisterEntity("&larrlp;", "\u21AB");
            RegisterEntity("&larrpl;", "\u2939");
            RegisterEntity("&larrsim;", "\u2973");
            RegisterEntity("&larrtl;", "\u21A2");
            RegisterEntity("&lat;", "\u2AAB");
            RegisterEntity("&lAtail;", "\u291B");
            RegisterEntity("&latail;", "\u2919");
            RegisterEntity("&late;", "\u2AAD");
            RegisterEntity("&lates;", "\u2AAD\uFE00");
            RegisterEntity("&lBarr;", "\u290E");
            RegisterEntity("&lbarr;", "\u290C");
            RegisterEntity("&lbbrk;", "\u2772");
            RegisterEntity("&lbrace;", "\u007B");
            RegisterEntity("&lbrack;", "\u005B");
            RegisterEntity("&lbrke;", "\u298B");
            RegisterEntity("&lbrksld;", "\u298F");
            RegisterEntity("&lbrkslu;", "\u298D");
            RegisterEntity("&Lcaron;", "\u013D");
            RegisterEntity("&lcaron;", "\u013E");
            RegisterEntity("&Lcedil;", "\u013B");
            RegisterEntity("&lcedil;", "\u013C");
            RegisterEntity("&lceil;", "\u2308");
            RegisterEntity("&lcub;", "\u007B");
            RegisterEntity("&Lcy;", "\u041B");
            RegisterEntity("&lcy;", "\u043B");
            RegisterEntity("&ldca;", "\u2936");
            RegisterEntity("&ldquo;", "\u201C");
            RegisterEntity("&ldquor;", "\u201E");
            RegisterEntity("&ldrdhar;", "\u2967");
            RegisterEntity("&ldrushar;", "\u294B");
            RegisterEntity("&ldsh;", "\u21B2");
            RegisterEntity("&lE;", "\u2266");
            RegisterEntity("&le;", "\u2264");
            RegisterEntity("&LeftAngleBracket;", "\u27E8");
            RegisterEntity("&LeftArrow;", "\u2190");
            RegisterEntity("&Leftarrow;", "\u21D0");
            RegisterEntity("&leftarrow;", "\u2190");
            RegisterEntity("&LeftArrowBar;", "\u21E4");
            RegisterEntity("&LeftArrowRightArrow;", "\u21C6");
            RegisterEntity("&leftarrowtail;", "\u21A2");
            RegisterEntity("&LeftCeiling;", "\u2308");
            RegisterEntity("&LeftDoubleBracket;", "\u27E6");
            RegisterEntity("&LeftDownTeeVector;", "\u2961");
            RegisterEntity("&LeftDownVector;", "\u21C3");
            RegisterEntity("&LeftDownVectorBar;", "\u2959");
            RegisterEntity("&LeftFloor;", "\u230A");
            RegisterEntity("&leftharpoondown;", "\u21BD");
            RegisterEntity("&leftharpoonup;", "\u21BC");
            RegisterEntity("&leftleftarrows;", "\u21C7");
            RegisterEntity("&LeftRightArrow;", "\u2194");
            RegisterEntity("&Leftrightarrow;", "\u21D4");
            RegisterEntity("&leftrightarrow;", "\u2194");
            RegisterEntity("&leftrightarrows;", "\u21C6");
            RegisterEntity("&leftrightharpoons;", "\u21CB");
            RegisterEntity("&leftrightsquigarrow;", "\u21AD");
            RegisterEntity("&LeftRightVector;", "\u294E");
            RegisterEntity("&LeftTee;", "\u22A3");
            RegisterEntity("&LeftTeeArrow;", "\u21A4");
            RegisterEntity("&LeftTeeVector;", "\u295A");
            RegisterEntity("&leftthreetimes;", "\u22CB");
            RegisterEntity("&LeftTriangle;", "\u22B2");
            RegisterEntity("&LeftTriangleBar;", "\u29CF");
            RegisterEntity("&LeftTriangleEqual;", "\u22B4");
            RegisterEntity("&LeftUpDownVector;", "\u2951");
            RegisterEntity("&LeftUpTeeVector;", "\u2960");
            RegisterEntity("&LeftUpVector;", "\u21BF");
            RegisterEntity("&LeftUpVectorBar;", "\u2958");
            RegisterEntity("&LeftVector;", "\u21BC");
            RegisterEntity("&LeftVectorBar;", "\u2952");
            RegisterEntity("&lEg;", "\u2A8B");
            RegisterEntity("&leg;", "\u22DA");
            RegisterEntity("&leq;", "\u2264");
            RegisterEntity("&leqq;", "\u2266");
            RegisterEntity("&leqslant;", "\u2A7D");
            RegisterEntity("&les;", "\u2A7D");
            RegisterEntity("&lescc;", "\u2AA8");
            RegisterEntity("&lesdot;", "\u2A7F");
            RegisterEntity("&lesdoto;", "\u2A81");
            RegisterEntity("&lesdotor;", "\u2A83");
            RegisterEntity("&lesg;", "\u22DA\uFE00");
            RegisterEntity("&lesges;", "\u2A93");
            RegisterEntity("&lessapprox;", "\u2A85");
            RegisterEntity("&lessdot;", "\u22D6");
            RegisterEntity("&lesseqgtr;", "\u22DA");
            RegisterEntity("&lesseqqgtr;", "\u2A8B");
            RegisterEntity("&LessEqualGreater;", "\u22DA");
            RegisterEntity("&LessFullEqual;", "\u2266");
            RegisterEntity("&LessGreater;", "\u2276");
            RegisterEntity("&lessgtr;", "\u2276");
            RegisterEntity("&LessLess;", "\u2AA1");
            RegisterEntity("&lesssim;", "\u2272");
            RegisterEntity("&LessSlantEqual;", "\u2A7D");
            RegisterEntity("&LessTilde;", "\u2272");
            RegisterEntity("&lfisht;", "\u297C");
            RegisterEntity("&lfloor;", "\u230A");
            RegisterEntity("&Lfr;", "\uD835\uDD0F");
            RegisterEntity("&lfr;", "\uD835\uDD29");
            RegisterEntity("&lg;", "\u2276");
            RegisterEntity("&lgE;", "\u2A91");
            RegisterEntity("&lHar;", "\u2962");
            RegisterEntity("&lhard;", "\u21BD");
            RegisterEntity("&lharu;", "\u21BC");
            RegisterEntity("&lharul;", "\u296A");
            RegisterEntity("&lhblk;", "\u2584");
            RegisterEntity("&LJcy;", "\u0409");
            RegisterEntity("&ljcy;", "\u0459");
            RegisterEntity("&Ll;", "\u22D8");
            RegisterEntity("&ll;", "\u226A");
            RegisterEntity("&llarr;", "\u21C7");
            RegisterEntity("&llcorner;", "\u231E");
            RegisterEntity("&Lleftarrow;", "\u21DA");
            RegisterEntity("&llhard;", "\u296B");
            RegisterEntity("&lltri;", "\u25FA");
            RegisterEntity("&Lmidot;", "\u013F");
            RegisterEntity("&lmidot;", "\u0140");
            RegisterEntity("&lmoust;", "\u23B0");
            RegisterEntity("&lmoustache;", "\u23B0");
            RegisterEntity("&lnap;", "\u2A89");
            RegisterEntity("&lnapprox;", "\u2A89");
            RegisterEntity("&lnE;", "\u2268");
            RegisterEntity("&lne;", "\u2A87");
            RegisterEntity("&lneq;", "\u2A87");
            RegisterEntity("&lneqq;", "\u2268");
            RegisterEntity("&lnsim;", "\u22E6");
            RegisterEntity("&loang;", "\u27EC");
            RegisterEntity("&loarr;", "\u21FD");
            RegisterEntity("&lobrk;", "\u27E6");
            RegisterEntity("&LongLeftArrow;", "\u27F5");
            RegisterEntity("&Longleftarrow;", "\u27F8");
            RegisterEntity("&longleftarrow;", "\u27F5");
            RegisterEntity("&LongLeftRightArrow;", "\u27F7");
            RegisterEntity("&Longleftrightarrow;", "\u27FA");
            RegisterEntity("&longleftrightarrow;", "\u27F7");
            RegisterEntity("&longmapsto;", "\u27FC");
            RegisterEntity("&LongRightArrow;", "\u27F6");
            RegisterEntity("&Longrightarrow;", "\u27F9");
            RegisterEntity("&longrightarrow;", "\u27F6");
            RegisterEntity("&looparrowleft;", "\u21AB");
            RegisterEntity("&looparrowright;", "\u21AC");
            RegisterEntity("&lopar;", "\u2985");
            RegisterEntity("&Lopf;", "\uD835\uDD43");
            RegisterEntity("&lopf;", "\uD835\uDD5D");
            RegisterEntity("&loplus;", "\u2A2D");
            RegisterEntity("&lotimes;", "\u2A34");
            RegisterEntity("&lowast;", "\u2217");
            RegisterEntity("&lowbar;", "\u005F");
            RegisterEntity("&LowerLeftArrow;", "\u2199");
            RegisterEntity("&LowerRightArrow;", "\u2198");
            RegisterEntity("&loz;", "\u25CA");
            RegisterEntity("&lozenge;", "\u25CA");
            RegisterEntity("&lozf;", "\u29EB");
            RegisterEntity("&lpar;", "\u0028");
            RegisterEntity("&lparlt;", "\u2993");
            RegisterEntity("&lrarr;", "\u21C6");
            RegisterEntity("&lrcorner;", "\u231F");
            RegisterEntity("&lrhar;", "\u21CB");
            RegisterEntity("&lrhard;", "\u296D");
            RegisterEntity("&lrm;", "\u200E");
            RegisterEntity("&lrtri;", "\u22BF");
            RegisterEntity("&lsaquo;", "\u2039");
            RegisterEntity("&Lscr;", "\u2112");
            RegisterEntity("&lscr;", "\uD835\uDCC1");
            RegisterEntity("&Lsh;", "\u21B0");
            RegisterEntity("&lsh;", "\u21B0");
            RegisterEntity("&lsim;", "\u2272");
            RegisterEntity("&lsime;", "\u2A8D");
            RegisterEntity("&lsimg;", "\u2A8F");
            RegisterEntity("&lsqb;", "\u005B");
            RegisterEntity("&lsquo;", "\u2018");
            RegisterEntity("&lsquor;", "\u201A");
            RegisterEntity("&Lstrok;", "\u0141");
            RegisterEntity("&lstrok;", "\u0142");
            RegisterEntity("&LT;", "\u003C");
            RegisterEntity("&Lt;", "\u226A");
            RegisterEntity("&lt;", "\u003C");
            RegisterEntity("&ltcc;", "\u2AA6");
            RegisterEntity("&ltcir;", "\u2A79");
            RegisterEntity("&ltdot;", "\u22D6");
            RegisterEntity("&lthree;", "\u22CB");
            RegisterEntity("&ltimes;", "\u22C9");
            RegisterEntity("&ltlarr;", "\u2976");
            RegisterEntity("&ltquest;", "\u2A7B");
            RegisterEntity("&ltri;", "\u25C3");
            RegisterEntity("&ltrie;", "\u22B4");
            RegisterEntity("&ltrif;", "\u25C2");
            RegisterEntity("&ltrPar;", "\u2996");
            RegisterEntity("&lurdshar;", "\u294A");
            RegisterEntity("&luruhar;", "\u2966");
            RegisterEntity("&lvertneqq;", "\u2268\uFE00");
            RegisterEntity("&lvnE;", "\u2268\uFE00");
            RegisterEntity("&macr;", "\u00AF");
            RegisterEntity("&male;", "\u2642");
            RegisterEntity("&malt;", "\u2720");
            RegisterEntity("&maltese;", "\u2720");
            RegisterEntity("&Map;", "\u2905");
            RegisterEntity("&map;", "\u21A6");
            RegisterEntity("&mapsto;", "\u21A6");
            RegisterEntity("&mapstodown;", "\u21A7");
            RegisterEntity("&mapstoleft;", "\u21A4");
            RegisterEntity("&mapstoup;", "\u21A5");
            RegisterEntity("&marker;", "\u25AE");
            RegisterEntity("&mcomma;", "\u2A29");
            RegisterEntity("&Mcy;", "\u041C");
            RegisterEntity("&mcy;", "\u043C");
            RegisterEntity("&mdash;", "\u2014");
            RegisterEntity("&mDDot;", "\u223A");
            RegisterEntity("&measuredangle;", "\u2221");
            RegisterEntity("&MediumSpace;", "\u205F");
            RegisterEntity("&Mellintrf;", "\u2133");
            RegisterEntity("&Mfr;", "\uD835\uDD10");
            RegisterEntity("&mfr;", "\uD835\uDD2A");
            RegisterEntity("&mho;", "\u2127");
            RegisterEntity("&micro;", "\u00B5");
            RegisterEntity("&mid;", "\u2223");
            RegisterEntity("&midast;", "\u002A");
            RegisterEntity("&midcir;", "\u2AF0");
            RegisterEntity("&middot;", "\u00B7");
            RegisterEntity("&minus;", "\u2212");
            RegisterEntity("&minusb;", "\u229F");
            RegisterEntity("&minusd;", "\u2238");
            RegisterEntity("&minusdu;", "\u2A2A");
            RegisterEntity("&MinusPlus;", "\u2213");
            RegisterEntity("&mlcp;", "\u2ADB");
            RegisterEntity("&mldr;", "\u2026");
            RegisterEntity("&mnplus;", "\u2213");
            RegisterEntity("&models;", "\u22A7");
            RegisterEntity("&Mopf;", "\uD835\uDD44");
            RegisterEntity("&mopf;", "\uD835\uDD5E");
            RegisterEntity("&mp;", "\u2213");
            RegisterEntity("&Mscr;", "\u2133");
            RegisterEntity("&mscr;", "\uD835\uDCC2");
            RegisterEntity("&mstpos;", "\u223E");
            RegisterEntity("&Mu;", "\u039C");
            RegisterEntity("&mu;", "\u03BC");
            RegisterEntity("&multimap;", "\u22B8");
            RegisterEntity("&mumap;", "\u22B8");
            RegisterEntity("&nabla;", "\u2207");
            RegisterEntity("&Nacute;", "\u0143");
            RegisterEntity("&nacute;", "\u0144");
            RegisterEntity("&nang;", "\u2220\u20D2");
            RegisterEntity("&nap;", "\u2249");
            RegisterEntity("&napE;", "\u2A70\u0338");
            RegisterEntity("&napid;", "\u224B\u0338");
            RegisterEntity("&napos;", "\u0149");
            RegisterEntity("&napprox;", "\u2249");
            RegisterEntity("&natur;", "\u266E");
            RegisterEntity("&natural;", "\u266E");
            RegisterEntity("&naturals;", "\u2115");
            RegisterEntity("&nbsp;", "\u00A0");
            RegisterEntity("&nbump;", "\u224E\u0338");
            RegisterEntity("&nbumpe;", "\u224F\u0338");
            RegisterEntity("&ncap;", "\u2A43");
            RegisterEntity("&Ncaron;", "\u0147");
            RegisterEntity("&ncaron;", "\u0148");
            RegisterEntity("&Ncedil;", "\u0145");
            RegisterEntity("&ncedil;", "\u0146");
            RegisterEntity("&ncong;", "\u2247");
            RegisterEntity("&ncongdot;", "\u2A6D\u0338");
            RegisterEntity("&ncup;", "\u2A42");
            RegisterEntity("&Ncy;", "\u041D");
            RegisterEntity("&ncy;", "\u043D");
            RegisterEntity("&ndash;", "\u2013");
            RegisterEntity("&ne;", "\u2260");
            RegisterEntity("&nearhk;", "\u2924");
            RegisterEntity("&neArr;", "\u21D7");
            RegisterEntity("&nearr;", "\u2197");
            RegisterEntity("&nearrow;", "\u2197");
            RegisterEntity("&nedot;", "\u2250\u0338");
            RegisterEntity("&NegativeMediumSpace;", "\u200B");
            RegisterEntity("&NegativeThickSpace;", "\u200B");
            RegisterEntity("&NegativeThinSpace;", "\u200B");
            RegisterEntity("&NegativeVeryThinSpace;", "\u200B");
            RegisterEntity("&nequiv;", "\u2262");
            RegisterEntity("&nesear;", "\u2928");
            RegisterEntity("&nesim;", "\u2242\u0338");
            RegisterEntity("&NestedGreaterGreater;", "\u226B");
            RegisterEntity("&NestedLessLess;", "\u226A");
            RegisterEntity("&NewLine;", "\u000A");
            RegisterEntity("&nexist;", "\u2204");
            RegisterEntity("&nexists;", "\u2204");
            RegisterEntity("&Nfr;", "\uD835\uDD11");
            RegisterEntity("&nfr;", "\uD835\uDD2B");
            RegisterEntity("&ngE;", "\u2267\u0338");
            RegisterEntity("&nge;", "\u2271");
            RegisterEntity("&ngeq;", "\u2271");
            RegisterEntity("&ngeqq;", "\u2267\u0338");
            RegisterEntity("&ngeqslant;", "\u2A7E\u0338");
            RegisterEntity("&nges;", "\u2A7E\u0338");
            RegisterEntity("&nGg;", "\u22D9\u0338");
            RegisterEntity("&ngsim;", "\u2275");
            RegisterEntity("&nGt;", "\u226B\u20D2");
            RegisterEntity("&ngt;", "\u226F");
            RegisterEntity("&ngtr;", "\u226F");
            RegisterEntity("&nGtv;", "\u226B\u0338");
            RegisterEntity("&nhArr;", "\u21CE");
            RegisterEntity("&nharr;", "\u21AE");
            RegisterEntity("&nhpar;", "\u2AF2");
            RegisterEntity("&ni;", "\u220B");
            RegisterEntity("&nis;", "\u22FC");
            RegisterEntity("&nisd;", "\u22FA");
            RegisterEntity("&niv;", "\u220B");
            RegisterEntity("&NJcy;", "\u040A");
            RegisterEntity("&njcy;", "\u045A");
            RegisterEntity("&nlArr;", "\u21CD");
            RegisterEntity("&nlarr;", "\u219A");
            RegisterEntity("&nldr;", "\u2025");
            RegisterEntity("&nlE;", "\u2266\u0338");
            RegisterEntity("&nle;", "\u2270");
            RegisterEntity("&nLeftarrow;", "\u21CD");
            RegisterEntity("&nleftarrow;", "\u219A");
            RegisterEntity("&nLeftrightarrow;", "\u21CE");
            RegisterEntity("&nleftrightarrow;", "\u21AE");
            RegisterEntity("&nleq;", "\u2270");
            RegisterEntity("&nleqq;", "\u2266\u0338");
            RegisterEntity("&nleqslant;", "\u2A7D\u0338");
            RegisterEntity("&nles;", "\u2A7D\u0338");
            RegisterEntity("&nless;", "\u226E");
            RegisterEntity("&nLl;", "\u22D8\u0338");
            RegisterEntity("&nlsim;", "\u2274");
            RegisterEntity("&nLt;", "\u226A\u20D2");
            RegisterEntity("&nlt;", "\u226E");
            RegisterEntity("&nltri;", "\u22EA");
            RegisterEntity("&nltrie;", "\u22EC");
            RegisterEntity("&nLtv;", "\u226A\u0338");
            RegisterEntity("&nmid;", "\u2224");
            RegisterEntity("&NoBreak;", "\u2060");
            RegisterEntity("&NonBreakingSpace;", "\u00A0");
            RegisterEntity("&Nopf;", "\u2115");
            RegisterEntity("&nopf;", "\uD835\uDD5F");
            RegisterEntity("&Not;", "\u2AEC");
            RegisterEntity("&not;", "\u00AC");
            RegisterEntity("&NotCongruent;", "\u2262");
            RegisterEntity("&NotCupCap;", "\u226D");
            RegisterEntity("&NotDoubleVerticalBar;", "\u2226");
            RegisterEntity("&NotElement;", "\u2209");
            RegisterEntity("&NotEqual;", "\u2260");
            RegisterEntity("&NotEqualTilde;", "\u2242\u0338");
            RegisterEntity("&NotExists;", "\u2204");
            RegisterEntity("&NotGreater;", "\u226F");
            RegisterEntity("&NotGreaterEqual;", "\u2271");
            RegisterEntity("&NotGreaterFullEqual;", "\u2267\u0338");
            RegisterEntity("&NotGreaterGreater;", "\u226B\u0338");
            RegisterEntity("&NotGreaterLess;", "\u2279");
            RegisterEntity("&NotGreaterSlantEqual;", "\u2A7E\u0338");
            RegisterEntity("&NotGreaterTilde;", "\u2275");
            RegisterEntity("&NotHumpDownHump;",  "\u224E\u0338");
            RegisterEntity("&NotHumpEqual;",  "\u224F\u0338");
            RegisterEntity("&notin;", "\u2209");
            RegisterEntity("&notindot;",  "\u22F5\u0338");
            RegisterEntity("&notinE;", "\u22F9\u0338");
            RegisterEntity("&notinva;", "\u2209");
            RegisterEntity("&notinvb;", "\u22F7");
            RegisterEntity("&notinvc;", "\u22F6");
            RegisterEntity("&NotLeftTriangle;", "\u22EA");
            RegisterEntity("&NotLeftTriangleBar;", "\u29CF\u0338");
            RegisterEntity("&NotLeftTriangleEqual;", "\u22EC");
            RegisterEntity("&NotLess;", "\u226E");
            RegisterEntity("&NotLessEqual;", "\u2270");
            RegisterEntity("&NotLessGreater;", "\u2278");
            RegisterEntity("&NotLessLess;", "\u226A\u0338");
            RegisterEntity("&NotLessSlantEqual;", "\u2A7D\u0338");
            RegisterEntity("&NotLessTilde;", "\u2274");
            RegisterEntity("&NotNestedGreaterGreater;", "\u2AA2\u0338");
            RegisterEntity("&NotNestedLessLess;", "\u2AA1\u0338");
            RegisterEntity("&notni;", "\u220C");
            RegisterEntity("&notniva;", "\u220C");
            RegisterEntity("&notnivb;", "\u22FE");
            RegisterEntity("&notnivc;", "\u22FD");
            RegisterEntity("&NotPrecedes;", "\u2280");
            RegisterEntity("&NotPrecedesEqual;", "\u2AAF\u0338");
            RegisterEntity("&NotPrecedesSlantEqual;", "\u22E0");
            RegisterEntity("&NotReverseElement;", "\u220C");
            RegisterEntity("&NotRightTriangle;", "\u22EB");
            RegisterEntity("&NotRightTriangleBar;", "\u29D0\u0338");
            RegisterEntity("&NotRightTriangleEqual;", "\u22ED");
            RegisterEntity("&NotSquareSubset;", "\u228F\u0338");
            RegisterEntity("&NotSquareSubsetEqual;", "\u22E2");
            RegisterEntity("&NotSquareSuperset;", "\u2290\u0338");
            RegisterEntity("&NotSquareSupersetEqual;", "\u22E3");
            RegisterEntity("&NotSubset;", "\u2282\u20D2");
            RegisterEntity("&NotSubsetEqual;", "\u2288");
            RegisterEntity("&NotSucceeds;", "\u2281");
            RegisterEntity("&NotSucceedsEqual;", "\u2AB0\u0338");
            RegisterEntity("&NotSucceedsSlantEqual;", "\u22E1");
            RegisterEntity("&NotSucceedsTilde;", "\u227F\u0338");
            RegisterEntity("&NotSuperset;", "\u2283\u20D2");
            RegisterEntity("&NotSupersetEqual;", "\u2289");
            RegisterEntity("&NotTilde;", "\u2241");
            RegisterEntity("&NotTildeEqual;", "\u2244");
            RegisterEntity("&NotTildeFullEqual;", "\u2247");
            RegisterEntity("&NotTildeTilde;", "\u2249");
            RegisterEntity("&NotVerticalBar;", "\u2224");
            RegisterEntity("&npar;", "\u2226");
            RegisterEntity("&nparallel;", "\u2226");
            RegisterEntity("&nparsl;", "\u2AFD\u20E5");
            RegisterEntity("&npart;", "\u2202\u0338");
            RegisterEntity("&npolint;", "\u2A14");
            RegisterEntity("&npr;", "\u2280");
            RegisterEntity("&nprcue;", "\u22E0");
            RegisterEntity("&npre;", "\u2AAF\u0338");
            RegisterEntity("&nprec;", "\u2280");
            RegisterEntity("&npreceq;", "\u2AAF\u0338");
            RegisterEntity("&nrArr;", "\u21CF");
            RegisterEntity("&nrarr;", "\u219B");
            RegisterEntity("&nrarrc;", "\u2933\u0338");
            RegisterEntity("&nrarrw;", "\u219D\u0338");
            RegisterEntity("&nRightarrow;", "\u21CF");
            RegisterEntity("&nrightarrow;", "\u219B");
            RegisterEntity("&nrtri;", "\u22EB");
            RegisterEntity("&nrtrie;", "\u22ED");
            RegisterEntity("&nsc;", "\u2281");
            RegisterEntity("&nsccue;", "\u22E1");
            RegisterEntity("&nsce;", "\u2AB0\u0338");
            RegisterEntity("&Nscr;", "\uD835\uDCA9");
            RegisterEntity("&nscr;", "\uD835\uDCC3");
            RegisterEntity("&nshortmid;", "\u2224");
            RegisterEntity("&nshortparallel;", "\u2226");
            RegisterEntity("&nsim;", "\u2241");
            RegisterEntity("&nsime;", "\u2244");
            RegisterEntity("&nsimeq;", "\u2244");
            RegisterEntity("&nsmid;", "\u2224");
            RegisterEntity("&nspar;", "\u2226");
            RegisterEntity("&nsqsube;", "\u22E2");
            RegisterEntity("&nsqsupe;", "\u22E3");
            RegisterEntity("&nsub;", "\u2284");
            RegisterEntity("&nsubE;",  "\u2AC5\u0338");
            RegisterEntity("&nsube;", "\u2288");
            RegisterEntity("&nsubset;",  "\u2282\u20D2");
            RegisterEntity("&nsubseteq;", "\u2288");
            RegisterEntity("&nsubseteqq;",  "\u2AC5\u0338");
            RegisterEntity("&nsucc;", "\u2281");
            RegisterEntity("&nsucceq;",  "\u2AB0\u0338");
            RegisterEntity("&nsup;", "\u2285");
            RegisterEntity("&nsupE;",  "\u2AC6\u0338");
            RegisterEntity("&nsupe;", "\u2289");
            RegisterEntity("&nsupset;",  "\u2283\u20D2");
            RegisterEntity("&nsupseteq;", "\u2289");
            RegisterEntity("&nsupseteqq;",  "\u2AC6\u0338");
            RegisterEntity("&ntgl;", "\u2279");
            RegisterEntity("&Ntilde;", "\u00D1");
            RegisterEntity("&ntilde;", "\u00F1");
            RegisterEntity("&ntlg;", "\u2278");
            RegisterEntity("&ntriangleleft;", "\u22EA");
            RegisterEntity("&ntrianglelefteq;", "\u22EC");
            RegisterEntity("&ntriangleright;", "\u22EB");
            RegisterEntity("&ntrianglerighteq;", "\u22ED");
            RegisterEntity("&Nu;", "\u039D");
            RegisterEntity("&nu;", "\u03BD");
            RegisterEntity("&num;", "\u0023");
            RegisterEntity("&numero;", "\u2116");
            RegisterEntity("&numsp;", "\u2007");
            RegisterEntity("&nvap;",  "\u224D\u20D2");
            RegisterEntity("&nVDash;", "\u22AF");
            RegisterEntity("&nVdash;", "\u22AE");
            RegisterEntity("&nvDash;", "\u22AD");
            RegisterEntity("&nvdash;", "\u22AC");
            RegisterEntity("&nvge;",  "\u2265\u20D2");
            RegisterEntity("&nvgt;",  "\u003E\u20D2");
            RegisterEntity("&nvHarr;", "\u2904");
            RegisterEntity("&nvinfin;", "\u29DE");
            RegisterEntity("&nvlArr;", "\u2902");
            RegisterEntity("&nvle;",  "\u2264\u20D2");
            RegisterEntity("&nvlt;",  "\u003C\u20D2");
            RegisterEntity("&nvltrie;",  "\u22B4\u20D2");
            RegisterEntity("&nvrArr;", "\u2903");
            RegisterEntity("&nvrtrie;",  "\u22B5\u20D2");
            RegisterEntity("&nvsim;",  "\u223C\u20D2");
            RegisterEntity("&nwarhk;", "\u2923");
            RegisterEntity("&nwArr;", "\u21D6");
            RegisterEntity("&nwarr;", "\u2196");
            RegisterEntity("&nwarrow;", "\u2196");
            RegisterEntity("&nwnear;", "\u2927");
            RegisterEntity("&Oacute;", "\u00D3");
            RegisterEntity("&oacute;", "\u00F3");
            RegisterEntity("&oast;", "\u229B");
            RegisterEntity("&ocir;", "\u229A");
            RegisterEntity("&Ocirc;", "\u00D4");
            RegisterEntity("&ocirc;", "\u00F4");
            RegisterEntity("&Ocy;", "\u041E");
            RegisterEntity("&ocy;", "\u043E");
            RegisterEntity("&odash;", "\u229D");
            RegisterEntity("&Odblac;", "\u0150");
            RegisterEntity("&odblac;", "\u0151");
            RegisterEntity("&odiv;", "\u2A38");
            RegisterEntity("&odot;", "\u2299");
            RegisterEntity("&odsold;", "\u29BC");
            RegisterEntity("&OElig;", "\u0152");
            RegisterEntity("&oelig;", "\u0153");
            RegisterEntity("&ofcir;", "\u29BF");
            RegisterEntity("&Ofr;", "\uD835\uDD12");
            RegisterEntity("&ofr;", "\uD835\uDD2C");
            RegisterEntity("&ogon;", "\u02DB");
            RegisterEntity("&Ograve;", "\u00D2");
            RegisterEntity("&ograve;", "\u00F2");
            RegisterEntity("&ogt;", "\u29C1");
            RegisterEntity("&ohbar;", "\u29B5");
            RegisterEntity("&ohm;", "\u03A9");
            RegisterEntity("&oint;", "\u222E");
            RegisterEntity("&olarr;", "\u21BA");
            RegisterEntity("&olcir;", "\u29BE");
            RegisterEntity("&olcross;", "\u29BB");
            RegisterEntity("&oline;", "\u203E");
            RegisterEntity("&olt;", "\u29C0");
            RegisterEntity("&Omacr;", "\u014C");
            RegisterEntity("&omacr;", "\u014D");
            RegisterEntity("&Omega;", "\u03A9");
            RegisterEntity("&omega;", "\u03C9");
            RegisterEntity("&Omicron;", "\u039F");
            RegisterEntity("&omicron;", "\u03BF");
            RegisterEntity("&omid;", "\u29B6");
            RegisterEntity("&ominus;", "\u2296");
            RegisterEntity("&Oopf;", "\uD835\uDD46");
            RegisterEntity("&oopf;", "\uD835\uDD60");
            RegisterEntity("&opar;", "\u29B7");
            RegisterEntity("&OpenCurlyDoubleQuote;", "\u201C");
            RegisterEntity("&OpenCurlyQuote;", "\u2018");
            RegisterEntity("&operp;", "\u29B9");
            RegisterEntity("&oplus;", "\u2295");
            RegisterEntity("&Or;", "\u2A54");
            RegisterEntity("&or;", "\u2228");
            RegisterEntity("&orarr;", "\u21BB");
            RegisterEntity("&ord;", "\u2A5D");
            RegisterEntity("&order;", "\u2134");
            RegisterEntity("&orderof;", "\u2134");
            RegisterEntity("&ordf;", "\u00AA");
            RegisterEntity("&ordm;", "\u00BA");
            RegisterEntity("&origof;", "\u22B6");
            RegisterEntity("&oror;", "\u2A56");
            RegisterEntity("&orslope;", "\u2A57");
            RegisterEntity("&orv;", "\u2A5B");
            RegisterEntity("&oS;", "\u24C8");
            RegisterEntity("&Oscr;", "\uD835\uDCAA");
            RegisterEntity("&oscr;", "\u2134");
            RegisterEntity("&Oslash;", "\u00D8");
            RegisterEntity("&oslash;", "\u00F8");
            RegisterEntity("&osol;", "\u2298");
            RegisterEntity("&Otilde;", "\u00D5");
            RegisterEntity("&otilde;", "\u00F5");
            RegisterEntity("&Otimes;", "\u2A37");
            RegisterEntity("&otimes;", "\u2297");
            RegisterEntity("&otimesas;", "\u2A36");
            RegisterEntity("&Ouml;", "\u00D6");
            RegisterEntity("&ouml;", "\u00F6");
            RegisterEntity("&ovbar;", "\u233D");
            RegisterEntity("&OverBar;", "\u203E");
            RegisterEntity("&OverBrace;", "\u23DE");
            RegisterEntity("&OverBracket;", "\u23B4");
            RegisterEntity("&OverParenthesis;", "\u23DC");
            RegisterEntity("&par;", "\u2225");
            RegisterEntity("&para;", "\u00B6");
            RegisterEntity("&parallel;", "\u2225");
            RegisterEntity("&parsim;", "\u2AF3");
            RegisterEntity("&parsl;", "\u2AFD");
            RegisterEntity("&part;", "\u2202");
            RegisterEntity("&PartialD;", "\u2202");
            RegisterEntity("&Pcy;", "\u041F");
            RegisterEntity("&pcy;", "\u043F");
            RegisterEntity("&percnt;", "\u0025");
            RegisterEntity("&period;", "\u002E");
            RegisterEntity("&permil;", "\u2030");
            RegisterEntity("&perp;", "\u22A5");
            RegisterEntity("&pertenk;", "\u2031");
            RegisterEntity("&Pfr;", "\uD835\uDD13");
            RegisterEntity("&pfr;", "\uD835\uDD2D");
            RegisterEntity("&Phi;", "\u03A6");
            RegisterEntity("&phi;", "\u03C6");
            RegisterEntity("&phiv;", "\u03D5");
            RegisterEntity("&phmmat;", "\u2133");
            RegisterEntity("&phone;", "\u260E");
            RegisterEntity("&Pi;", "\u03A0");
            RegisterEntity("&pi;", "\u03C0");
            RegisterEntity("&pitchfork;", "\u22D4");
            RegisterEntity("&piv;", "\u03D6");
            RegisterEntity("&planck;", "\u210F");
            RegisterEntity("&planckh;", "\u210E");
            RegisterEntity("&plankv;", "\u210F");
            RegisterEntity("&plus;", "\u002B");
            RegisterEntity("&plusacir;", "\u2A23");
            RegisterEntity("&plusb;", "\u229E");
            RegisterEntity("&pluscir;", "\u2A22");
            RegisterEntity("&plusdo;", "\u2214");
            RegisterEntity("&plusdu;", "\u2A25");
            RegisterEntity("&pluse;", "\u2A72");
            RegisterEntity("&PlusMinus;", "\u00B1");
            RegisterEntity("&plusmn;", "\u00B1");
            RegisterEntity("&plussim;", "\u2A26");
            RegisterEntity("&plustwo;", "\u2A27");
            RegisterEntity("&pm;", "\u00B1");
            RegisterEntity("&Poincareplane;", "\u210C");
            RegisterEntity("&pointint;", "\u2A15");
            RegisterEntity("&Popf;", "\u2119");
            RegisterEntity("&popf;", "\uD835\uDD61");
            RegisterEntity("&pound;", "\u00A3");
            RegisterEntity("&Pr;", "\u2ABB");
            RegisterEntity("&pr;", "\u227A");
            RegisterEntity("&prap;", "\u2AB7");
            RegisterEntity("&prcue;", "\u227C");
            RegisterEntity("&prE;", "\u2AB3");
            RegisterEntity("&pre;", "\u2AAF");
            RegisterEntity("&prec;", "\u227A");
            RegisterEntity("&precapprox;", "\u2AB7");
            RegisterEntity("&preccurlyeq;", "\u227C");
            RegisterEntity("&Precedes;", "\u227A");
            RegisterEntity("&PrecedesEqual;", "\u2AAF");
            RegisterEntity("&PrecedesSlantEqual;", "\u227C");
            RegisterEntity("&PrecedesTilde;", "\u227E");
            RegisterEntity("&preceq;", "\u2AAF");
            RegisterEntity("&precnapprox;", "\u2AB9");
            RegisterEntity("&precneqq;", "\u2AB5");
            RegisterEntity("&precnsim;", "\u22E8");
            RegisterEntity("&precsim;", "\u227E");
            RegisterEntity("&Prime;", "\u2033");
            RegisterEntity("&prime;", "\u2032");
            RegisterEntity("&primes;", "\u2119");
            RegisterEntity("&prnap;", "\u2AB9");
            RegisterEntity("&prnE;", "\u2AB5");
            RegisterEntity("&prnsim;", "\u22E8");
            RegisterEntity("&prod;", "\u220F");
            RegisterEntity("&Product;", "\u220F");
            RegisterEntity("&profalar;", "\u232E");
            RegisterEntity("&profline;", "\u2312");
            RegisterEntity("&profsurf;", "\u2313");
            RegisterEntity("&prop;", "\u221D");
            RegisterEntity("&Proportion;", "\u2237");
            RegisterEntity("&Proportional;", "\u221D");
            RegisterEntity("&propto;", "\u221D");
            RegisterEntity("&prsim;", "\u227E");
            RegisterEntity("&prurel;", "\u22B0");
            RegisterEntity("&Pscr;", "\uD835\uDCAB");
            RegisterEntity("&pscr;", "\uD835\uDCC5");
            RegisterEntity("&Psi;", "\u03A8");
            RegisterEntity("&psi;", "\u03C8");
            RegisterEntity("&puncsp;", "\u2008");
            RegisterEntity("&Qfr;", "\uD835\uDD14");
            RegisterEntity("&qfr;", "\uD835\uDD2E");
            RegisterEntity("&qint;", "\u2A0C");
            RegisterEntity("&Qopf;", "\u211A");
            RegisterEntity("&qopf;", "\uD835\uDD62");
            RegisterEntity("&qprime;", "\u2057");
            RegisterEntity("&Qscr;", "\uD835\uDCAC");
            RegisterEntity("&qscr;", "\uD835\uDCC6");
            RegisterEntity("&quaternions;", "\u210D");
            RegisterEntity("&quatint;", "\u2A16");
            RegisterEntity("&quest;", "\u003F");
            RegisterEntity("&questeq;", "\u225F");
            RegisterEntity("&QUOT;", "\u0022");
            RegisterEntity("&quot;", "\u0022");
            RegisterEntity("&rAarr;", "\u21DB");
            RegisterEntity("&race;",  "\u223D\u0331");
            RegisterEntity("&Racute;", "\u0154");
            RegisterEntity("&racute;", "\u0155");
            RegisterEntity("&radic;", "\u221A");
            RegisterEntity("&raemptyv;", "\u29B3");
            RegisterEntity("&Rang;", "\u27EB");
            RegisterEntity("&rang;", "\u27E9");
            RegisterEntity("&rangd;", "\u2992");
            RegisterEntity("&range;", "\u29A5");
            RegisterEntity("&rangle;", "\u27E9");
            RegisterEntity("&raquo;", "\u00BB");
            RegisterEntity("&Rarr;", "\u21A0");
            RegisterEntity("&rArr;", "\u21D2");
            RegisterEntity("&rarr;", "\u2192");
            RegisterEntity("&rarrap;", "\u2975");
            RegisterEntity("&rarrb;", "\u21E5");
            RegisterEntity("&rarrbfs;", "\u2920");
            RegisterEntity("&rarrc;", "\u2933");
            RegisterEntity("&rarrfs;", "\u291E");
            RegisterEntity("&rarrhk;", "\u21AA");
            RegisterEntity("&rarrlp;", "\u21AC");
            RegisterEntity("&rarrpl;", "\u2945");
            RegisterEntity("&rarrsim;", "\u2974");
            RegisterEntity("&Rarrtl;", "\u2916");
            RegisterEntity("&rarrtl;", "\u21A3");
            RegisterEntity("&rarrw;", "\u219D");
            RegisterEntity("&rAtail;", "\u291C");
            RegisterEntity("&ratail;", "\u291A");
            RegisterEntity("&ratio;", "\u2236");
            RegisterEntity("&rationals;", "\u211A");
            RegisterEntity("&RBarr;", "\u2910");
            RegisterEntity("&rBarr;", "\u290F");
            RegisterEntity("&rbarr;", "\u290D");
            RegisterEntity("&rbbrk;", "\u2773");
            RegisterEntity("&rbrace;", "\u007D");
            RegisterEntity("&rbrack;", "\u005D");
            RegisterEntity("&rbrke;", "\u298C");
            RegisterEntity("&rbrksld;", "\u298E");
            RegisterEntity("&rbrkslu;", "\u2990");
            RegisterEntity("&Rcaron;", "\u0158");
            RegisterEntity("&rcaron;", "\u0159");
            RegisterEntity("&Rcedil;", "\u0156");
            RegisterEntity("&rcedil;", "\u0157");
            RegisterEntity("&rceil;", "\u2309");
            RegisterEntity("&rcub;", "\u007D");
            RegisterEntity("&Rcy;", "\u0420");
            RegisterEntity("&rcy;", "\u0440");
            RegisterEntity("&rdca;", "\u2937");
            RegisterEntity("&rdldhar;", "\u2969");
            RegisterEntity("&rdquo;", "\u201D");
            RegisterEntity("&rdquor;", "\u201D");
            RegisterEntity("&rdsh;", "\u21B3");
            RegisterEntity("&Re;", "\u211C");
            RegisterEntity("&real;", "\u211C");
            RegisterEntity("&realine;", "\u211B");
            RegisterEntity("&realpart;", "\u211C");
            RegisterEntity("&reals;", "\u211D");
            RegisterEntity("&rect;", "\u25AD");
            RegisterEntity("&REG;", "\u00AE");
            RegisterEntity("&reg;", "\u00AE");
            RegisterEntity("&ReverseElement;", "\u220B");
            RegisterEntity("&ReverseEquilibrium;", "\u21CB");
            RegisterEntity("&ReverseUpEquilibrium;", "\u296F");
            RegisterEntity("&rfisht;", "\u297D");
            RegisterEntity("&rfloor;", "\u230B");
            RegisterEntity("&Rfr;", "\u211C");
            RegisterEntity("&rfr;", "\uD835\uDD2F");
            RegisterEntity("&rHar;", "\u2964");
            RegisterEntity("&rhard;", "\u21C1");
            RegisterEntity("&rharu;", "\u21C0");
            RegisterEntity("&rharul;", "\u296C");
            RegisterEntity("&Rho;", "\u03A1");
            RegisterEntity("&rho;", "\u03C1");
            RegisterEntity("&rhov;", "\u03F1");
            RegisterEntity("&RightAngleBracket;", "\u27E9");
            RegisterEntity("&RightArrow;", "\u2192");
            RegisterEntity("&Rightarrow;", "\u21D2");
            RegisterEntity("&rightarrow;", "\u2192");
            RegisterEntity("&RightArrowBar;", "\u21E5");
            RegisterEntity("&RightArrowLeftArrow;", "\u21C4");
            RegisterEntity("&rightarrowtail;", "\u21A3");
            RegisterEntity("&RightCeiling;", "\u2309");
            RegisterEntity("&RightDoubleBracket;", "\u27E7");
            RegisterEntity("&RightDownTeeVector;", "\u295D");
            RegisterEntity("&RightDownVector;", "\u21C2");
            RegisterEntity("&RightDownVectorBar;", "\u2955");
            RegisterEntity("&RightFloor;", "\u230B");
            RegisterEntity("&rightharpoondown;", "\u21C1");
            RegisterEntity("&rightharpoonup;", "\u21C0");
            RegisterEntity("&rightleftarrows;", "\u21C4");
            RegisterEntity("&rightleftharpoons;", "\u21CC");
            RegisterEntity("&rightrightarrows;", "\u21C9");
            RegisterEntity("&rightsquigarrow;", "\u219D");
            RegisterEntity("&RightTee;", "\u22A2");
            RegisterEntity("&RightTeeArrow;", "\u21A6");
            RegisterEntity("&RightTeeVector;", "\u295B");
            RegisterEntity("&rightthreetimes;", "\u22CC");
            RegisterEntity("&RightTriangle;", "\u22B3");
            RegisterEntity("&RightTriangleBar;", "\u29D0");
            RegisterEntity("&RightTriangleEqual;", "\u22B5");
            RegisterEntity("&RightUpDownVector;", "\u294F");
            RegisterEntity("&RightUpTeeVector;", "\u295C");
            RegisterEntity("&RightUpVector;", "\u21BE");
            RegisterEntity("&RightUpVectorBar;", "\u2954");
            RegisterEntity("&RightVector;", "\u21C0");
            RegisterEntity("&RightVectorBar;", "\u2953");
            RegisterEntity("&ring;", "\u02DA");
            RegisterEntity("&risingdotseq;", "\u2253");
            RegisterEntity("&rlarr;", "\u21C4");
            RegisterEntity("&rlhar;", "\u21CC");
            RegisterEntity("&rlm;", "\u200F");
            RegisterEntity("&rmoust;", "\u23B1");
            RegisterEntity("&rmoustache;", "\u23B1");
            RegisterEntity("&rnmid;", "\u2AEE");
            RegisterEntity("&roang;", "\u27ED");
            RegisterEntity("&roarr;", "\u21FE");
            RegisterEntity("&robrk;", "\u27E7");
            RegisterEntity("&ropar;", "\u2986");
            RegisterEntity("&Ropf;", "\u211D");
            RegisterEntity("&ropf;", "\uD835\uDD63");
            RegisterEntity("&roplus;", "\u2A2E");
            RegisterEntity("&rotimes;", "\u2A35");
            RegisterEntity("&RoundImplies;", "\u2970");
            RegisterEntity("&rpar;", "\u0029");
            RegisterEntity("&rpargt;", "\u2994");
            RegisterEntity("&rppolint;", "\u2A12");
            RegisterEntity("&rrarr;", "\u21C9");
            RegisterEntity("&Rrightarrow;", "\u21DB");
            RegisterEntity("&rsaquo;", "\u203A");
            RegisterEntity("&Rscr;", "\u211B");
            RegisterEntity("&rscr;", "\uD835\uDCC7");
            RegisterEntity("&Rsh;", "\u21B1");
            RegisterEntity("&rsh;", "\u21B1");
            RegisterEntity("&rsqb;", "\u005D");
            RegisterEntity("&rsquo;", "\u2019");
            RegisterEntity("&rsquor;", "\u2019");
            RegisterEntity("&rthree;", "\u22CC");
            RegisterEntity("&rtimes;", "\u22CA");
            RegisterEntity("&rtri;", "\u25B9");
            RegisterEntity("&rtrie;", "\u22B5");
            RegisterEntity("&rtrif;", "\u25B8");
            RegisterEntity("&rtriltri;", "\u29CE");
            RegisterEntity("&RuleDelayed;", "\u29F4");
            RegisterEntity("&ruluhar;", "\u2968");
            RegisterEntity("&rx;", "\u211E");
            RegisterEntity("&Sacute;", "\u015A");
            RegisterEntity("&sacute;", "\u015B");
            RegisterEntity("&sbquo;", "\u201A");
            RegisterEntity("&Sc;", "\u2ABC");
            RegisterEntity("&sc;", "\u227B");
            RegisterEntity("&scap;", "\u2AB8");
            RegisterEntity("&Scaron;", "\u0160");
            RegisterEntity("&scaron;", "\u0161");
            RegisterEntity("&sccue;", "\u227D");
            RegisterEntity("&scE;", "\u2AB4");
            RegisterEntity("&sce;", "\u2AB0");
            RegisterEntity("&Scedil;", "\u015E");
            RegisterEntity("&scedil;", "\u015F");
            RegisterEntity("&Scirc;", "\u015C");
            RegisterEntity("&scirc;", "\u015D");
            RegisterEntity("&scnap;", "\u2ABA");
            RegisterEntity("&scnE;", "\u2AB6");
            RegisterEntity("&scnsim;", "\u22E9");
            RegisterEntity("&scpolint;", "\u2A13");
            RegisterEntity("&scsim;", "\u227F");
            RegisterEntity("&Scy;", "\u0421");
            RegisterEntity("&scy;", "\u0441");
            RegisterEntity("&sdot;", "\u22C5");
            RegisterEntity("&sdotb;", "\u22A1");
            RegisterEntity("&sdote;", "\u2A66");
            RegisterEntity("&searhk;", "\u2925");
            RegisterEntity("&seArr;", "\u21D8");
            RegisterEntity("&searr;", "\u2198");
            RegisterEntity("&searrow;", "\u2198");
            RegisterEntity("&sect;", "\u00A7");
            RegisterEntity("&semi;", "\u003B");
            RegisterEntity("&seswar;", "\u2929");
            RegisterEntity("&setminus;", "\u2216");
            RegisterEntity("&setmn;", "\u2216");
            RegisterEntity("&sext;", "\u2736");
            RegisterEntity("&Sfr;", "\uD835\uDD16");
            RegisterEntity("&sfr;", "\uD835\uDD30");
            RegisterEntity("&sfrown;", "\u2322");
            RegisterEntity("&sharp;", "\u266F");
            RegisterEntity("&SHCHcy;", "\u0429");
            RegisterEntity("&shchcy;", "\u0449");
            RegisterEntity("&SHcy;", "\u0428");
            RegisterEntity("&shcy;", "\u0448");
            RegisterEntity("&ShortDownArrow;", "\u2193");
            RegisterEntity("&ShortLeftArrow;", "\u2190");
            RegisterEntity("&shortmid;", "\u2223");
            RegisterEntity("&shortparallel;", "\u2225");
            RegisterEntity("&ShortRightArrow;", "\u2192");
            RegisterEntity("&ShortUpArrow;", "\u2191");
            RegisterEntity("&shy;", "\u00AD");
            RegisterEntity("&Sigma;", "\u03A3");
            RegisterEntity("&sigma;", "\u03C3");
            RegisterEntity("&sigmaf;", "\u03C2");
            RegisterEntity("&sigmav;", "\u03C2");
            RegisterEntity("&sim;", "\u223C");
            RegisterEntity("&simdot;", "\u2A6A");
            RegisterEntity("&sime;", "\u2243");
            RegisterEntity("&simeq;", "\u2243");
            RegisterEntity("&simg;", "\u2A9E");
            RegisterEntity("&simgE;", "\u2AA0");
            RegisterEntity("&siml;", "\u2A9D");
            RegisterEntity("&simlE;", "\u2A9F");
            RegisterEntity("&simne;", "\u2246");
            RegisterEntity("&simplus;", "\u2A24");
            RegisterEntity("&simrarr;", "\u2972");
            RegisterEntity("&slarr;", "\u2190");
            RegisterEntity("&SmallCircle;", "\u2218");
            RegisterEntity("&smallsetminus;", "\u2216");
            RegisterEntity("&smashp;", "\u2A33");
            RegisterEntity("&smeparsl;", "\u29E4");
            RegisterEntity("&smid;", "\u2223");
            RegisterEntity("&smile;", "\u2323");
            RegisterEntity("&smt;", "\u2AAA");
            RegisterEntity("&smte;", "\u2AAC");
            RegisterEntity("&smtes;",  "\u2AAC\uFE00");
            RegisterEntity("&SOFTcy;", "\u042C");
            RegisterEntity("&softcy;", "\u044C");
            RegisterEntity("&sol;", "\u002F");
            RegisterEntity("&solb;", "\u29C4");
            RegisterEntity("&solbar;", "\u233F");
            RegisterEntity("&Sopf;", "\uD835\uDD4A");
            RegisterEntity("&sopf;", "\uD835\uDD64");
            RegisterEntity("&spades;", "\u2660");
            RegisterEntity("&spadesuit;", "\u2660");
            RegisterEntity("&spar;", "\u2225");
            RegisterEntity("&sqcap;", "\u2293");
            RegisterEntity("&sqcaps;",  "\u2293\uFE00");
            RegisterEntity("&sqcup;", "\u2294");
            RegisterEntity("&sqcups;",  "\u2294\uFE00");
            RegisterEntity("&Sqrt;", "\u221A");
            RegisterEntity("&sqsub;", "\u228F");
            RegisterEntity("&sqsube;", "\u2291");
            RegisterEntity("&sqsubset;", "\u228F");
            RegisterEntity("&sqsubseteq;", "\u2291");
            RegisterEntity("&sqsup;", "\u2290");
            RegisterEntity("&sqsupe;", "\u2292");
            RegisterEntity("&sqsupset;", "\u2290");
            RegisterEntity("&sqsupseteq;", "\u2292");
            RegisterEntity("&squ;", "\u25A1");
            RegisterEntity("&Square;", "\u25A1");
            RegisterEntity("&square;", "\u25A1");
            RegisterEntity("&SquareIntersection;", "\u2293");
            RegisterEntity("&SquareSubset;", "\u228F");
            RegisterEntity("&SquareSubsetEqual;", "\u2291");
            RegisterEntity("&SquareSuperset;", "\u2290");
            RegisterEntity("&SquareSupersetEqual;", "\u2292");
            RegisterEntity("&SquareUnion;", "\u2294");
            RegisterEntity("&squarf;", "\u25AA");
            RegisterEntity("&squf;", "\u25AA");
            RegisterEntity("&srarr;", "\u2192");
            RegisterEntity("&Sscr;", "\uD835\uDCAE");
            RegisterEntity("&sscr;", "\uD835\uDCC8");
            RegisterEntity("&ssetmn;", "\u2216");
            RegisterEntity("&ssmile;", "\u2323");
            RegisterEntity("&sstarf;", "\u22C6");
            RegisterEntity("&Star;", "\u22C6");
            RegisterEntity("&star;", "\u2606");
            RegisterEntity("&starf;", "\u2605");
            RegisterEntity("&straightepsilon;", "\u03F5");
            RegisterEntity("&straightphi;", "\u03D5");
            RegisterEntity("&strns;", "\u00AF");
            RegisterEntity("&Sub;", "\u22D0");
            RegisterEntity("&sub;", "\u2282");
            RegisterEntity("&subdot;", "\u2ABD");
            RegisterEntity("&subE;", "\u2AC5");
            RegisterEntity("&sube;", "\u2286");
            RegisterEntity("&subedot;", "\u2AC3");
            RegisterEntity("&submult;", "\u2AC1");
            RegisterEntity("&subnE;", "\u2ACB");
            RegisterEntity("&subne;", "\u228A");
            RegisterEntity("&subplus;", "\u2ABF");
            RegisterEntity("&subrarr;", "\u2979");
            RegisterEntity("&Subset;", "\u22D0");
            RegisterEntity("&subset;", "\u2282");
            RegisterEntity("&subseteq;", "\u2286");
            RegisterEntity("&subseteqq;", "\u2AC5");
            RegisterEntity("&SubsetEqual;", "\u2286");
            RegisterEntity("&subsetneq;", "\u228A");
            RegisterEntity("&subsetneqq;", "\u2ACB");
            RegisterEntity("&subsim;", "\u2AC7");
            RegisterEntity("&subsub;", "\u2AD5");
            RegisterEntity("&subsup;", "\u2AD3");
            RegisterEntity("&succ;", "\u227B");
            RegisterEntity("&succapprox;", "\u2AB8");
            RegisterEntity("&succcurlyeq;", "\u227D");
            RegisterEntity("&Succeeds;", "\u227B");
            RegisterEntity("&SucceedsEqual;", "\u2AB0");
            RegisterEntity("&SucceedsSlantEqual;", "\u227D");
            RegisterEntity("&SucceedsTilde;", "\u227F");
            RegisterEntity("&succeq;", "\u2AB0");
            RegisterEntity("&succnapprox;", "\u2ABA");
            RegisterEntity("&succneqq;", "\u2AB6");
            RegisterEntity("&succnsim;", "\u22E9");
            RegisterEntity("&succsim;", "\u227F");
            RegisterEntity("&SuchThat;", "\u220B");
            RegisterEntity("&Sum;", "\u2211");
            RegisterEntity("&sum;", "\u2211");
            RegisterEntity("&sung;", "\u266A");
            RegisterEntity("&Sup;", "\u22D1");
            RegisterEntity("&sup;", "\u2283");
            RegisterEntity("&sup1;", "\u00B9");
            RegisterEntity("&sup2;", "\u00B2");
            RegisterEntity("&sup3;", "\u00B3");
            RegisterEntity("&supdot;", "\u2ABE");
            RegisterEntity("&supdsub;", "\u2AD8");
            RegisterEntity("&supE;", "\u2AC6");
            RegisterEntity("&supe;", "\u2287");
            RegisterEntity("&supedot;", "\u2AC4");
            RegisterEntity("&Superset;", "\u2283");
            RegisterEntity("&SupersetEqual;", "\u2287");
            RegisterEntity("&suphsol;", "\u27C9");
            RegisterEntity("&suphsub;", "\u2AD7");
            RegisterEntity("&suplarr;", "\u297B");
            RegisterEntity("&supmult;", "\u2AC2");
            RegisterEntity("&supnE;", "\u2ACC");
            RegisterEntity("&supne;", "\u228B");
            RegisterEntity("&supplus;", "\u2AC0");
            RegisterEntity("&Supset;", "\u22D1");
            RegisterEntity("&supset;", "\u2283");
            RegisterEntity("&supseteq;", "\u2287");
            RegisterEntity("&supseteqq;", "\u2AC6");
            RegisterEntity("&supsetneq;", "\u228B");
            RegisterEntity("&supsetneqq;", "\u2ACC");
            RegisterEntity("&supsim;", "\u2AC8");
            RegisterEntity("&supsub;", "\u2AD4");
            RegisterEntity("&supsup;", "\u2AD6");
            RegisterEntity("&swarhk;", "\u2926");
            RegisterEntity("&swArr;", "\u21D9");
            RegisterEntity("&swarr;", "\u2199");
            RegisterEntity("&swarrow;", "\u2199");
            RegisterEntity("&swnwar;", "\u292A");
            RegisterEntity("&szlig;", "\u00DF");
            RegisterEntity("&Tab;", "\u0009");
            RegisterEntity("&target;", "\u2316");
            RegisterEntity("&Tau;", "\u03A4");
            RegisterEntity("&tau;", "\u03C4");
            RegisterEntity("&tbrk;", "\u23B4");
            RegisterEntity("&Tcaron;", "\u0164");
            RegisterEntity("&tcaron;", "\u0165");
            RegisterEntity("&Tcedil;", "\u0162");
            RegisterEntity("&tcedil;", "\u0163");
            RegisterEntity("&Tcy;", "\u0422");
            RegisterEntity("&tcy;", "\u0442");
            RegisterEntity("&tdot;", "\u20DB");
            RegisterEntity("&telrec;", "\u2315");
            RegisterEntity("&Tfr;", "\uD835\uDD17");
            RegisterEntity("&tfr;", "\uD835\uDD31");
            RegisterEntity("&there4;", "\u2234");
            RegisterEntity("&Therefore;", "\u2234");
            RegisterEntity("&therefore;", "\u2234");
            RegisterEntity("&Theta;", "\u0398");
            RegisterEntity("&theta;", "\u03B8");
            RegisterEntity("&thetasym;", "\u03D1");
            RegisterEntity("&thetav;", "\u03D1");
            RegisterEntity("&thickapprox;", "\u2248");
            RegisterEntity("&thicksim;", "\u223C");
            RegisterEntity("&ThickSpace;",  "\u205F\u200A");
            RegisterEntity("&thinsp;", "\u2009");
            RegisterEntity("&ThinSpace;", "\u2009");
            RegisterEntity("&thkap;", "\u2248");
            RegisterEntity("&thksim;", "\u223C");
            RegisterEntity("&THORN;", "\u00DE");
            RegisterEntity("&thorn;", "\u00FE");
            RegisterEntity("&Tilde;", "\u223C");
            RegisterEntity("&tilde;", "\u02DC");
            RegisterEntity("&TildeEqual;", "\u2243");
            RegisterEntity("&TildeFullEqual;", "\u2245");
            RegisterEntity("&TildeTilde;", "\u2248");
            RegisterEntity("&times;", "\u00D7");
            RegisterEntity("&timesb;", "\u22A0");
            RegisterEntity("&timesbar;", "\u2A31");
            RegisterEntity("&timesd;", "\u2A30");
            RegisterEntity("&tint;", "\u222D");
            RegisterEntity("&toea;", "\u2928");
            RegisterEntity("&top;", "\u22A4");
            RegisterEntity("&topbot;", "\u2336");
            RegisterEntity("&topcir;", "\u2AF1");
            RegisterEntity("&Topf;", "\uD835\uDD4B");
            RegisterEntity("&topf;", "\uD835\uDD65");
            RegisterEntity("&topfork;", "\u2ADA");
            RegisterEntity("&tosa;", "\u2929");
            RegisterEntity("&tprime;", "\u2034");
            RegisterEntity("&TRADE;", "\u2122");
            RegisterEntity("&trade;", "\u2122");
            RegisterEntity("&triangle;", "\u25B5");
            RegisterEntity("&triangledown;", "\u25BF");
            RegisterEntity("&triangleleft;", "\u25C3");
            RegisterEntity("&trianglelefteq;", "\u22B4");
            RegisterEntity("&triangleq;", "\u225C");
            RegisterEntity("&triangleright;", "\u25B9");
            RegisterEntity("&trianglerighteq;", "\u22B5");
            RegisterEntity("&tridot;", "\u25EC");
            RegisterEntity("&trie;", "\u225C");
            RegisterEntity("&triminus;", "\u2A3A");
            RegisterEntity("&TripleDot;", "\u20DB");
            RegisterEntity("&triplus;", "\u2A39");
            RegisterEntity("&trisb;", "\u29CD");
            RegisterEntity("&tritime;", "\u2A3B");
            RegisterEntity("&trpezium;", "\u23E2");
            RegisterEntity("&Tscr;", "\uD835\uDCAF");
            RegisterEntity("&tscr;", "\uD835\uDCC9");
            RegisterEntity("&TScy;", "\u0426");
            RegisterEntity("&tscy;", "\u0446");
            RegisterEntity("&TSHcy;", "\u040B");
            RegisterEntity("&tshcy;", "\u045B");
            RegisterEntity("&Tstrok;", "\u0166");
            RegisterEntity("&tstrok;", "\u0167");
            RegisterEntity("&twixt;", "\u226C");
            RegisterEntity("&twoheadleftarrow;", "\u219E");
            RegisterEntity("&twoheadrightarrow;", "\u21A0");
            RegisterEntity("&Uacute;", "\u00DA");
            RegisterEntity("&uacute;", "\u00FA");
            RegisterEntity("&Uarr;", "\u219F");
            RegisterEntity("&uArr;", "\u21D1");
            RegisterEntity("&uarr;", "\u2191");
            RegisterEntity("&Uarrocir;", "\u2949");
            RegisterEntity("&Ubrcy;", "\u040E");
            RegisterEntity("&ubrcy;", "\u045E");
            RegisterEntity("&Ubreve;", "\u016C");
            RegisterEntity("&ubreve;", "\u016D");
            RegisterEntity("&Ucirc;", "\u00DB");
            RegisterEntity("&ucirc;", "\u00FB");
            RegisterEntity("&Ucy;", "\u0423");
            RegisterEntity("&ucy;", "\u0443");
            RegisterEntity("&udarr;", "\u21C5");
            RegisterEntity("&Udblac;", "\u0170");
            RegisterEntity("&udblac;", "\u0171");
            RegisterEntity("&udhar;", "\u296E");
            RegisterEntity("&ufisht;", "\u297E");
            RegisterEntity("&Ufr;", "\uD835\uDD18");
            RegisterEntity("&ufr;", "\uD835\uDD32");
            RegisterEntity("&Ugrave;", "\u00D9");
            RegisterEntity("&ugrave;", "\u00F9");
            RegisterEntity("&uHar;", "\u2963");
            RegisterEntity("&uharl;", "\u21BF");
            RegisterEntity("&uharr;", "\u21BE");
            RegisterEntity("&uhblk;", "\u2580");
            RegisterEntity("&ulcorn;", "\u231C");
            RegisterEntity("&ulcorner;", "\u231C");
            RegisterEntity("&ulcrop;", "\u230F");
            RegisterEntity("&ultri;", "\u25F8");
            RegisterEntity("&Umacr;", "\u016A");
            RegisterEntity("&umacr;", "\u016B");
            RegisterEntity("&uml;", "\u00A8");
            RegisterEntity("&UnderBar;", "\u005F");
            RegisterEntity("&UnderBrace;", "\u23DF");
            RegisterEntity("&UnderBracket;", "\u23B5");
            RegisterEntity("&UnderParenthesis;", "\u23DD");
            RegisterEntity("&Union;", "\u22C3");
            RegisterEntity("&UnionPlus;", "\u228E");
            RegisterEntity("&Uogon;", "\u0172");
            RegisterEntity("&uogon;", "\u0173");
            RegisterEntity("&Uopf;", "\uD835\uDD4C");
            RegisterEntity("&uopf;", "\uD835\uDD66");
            RegisterEntity("&UpArrow;", "\u2191");
            RegisterEntity("&Uparrow;", "\u21D1");
            RegisterEntity("&uparrow;", "\u2191");
            RegisterEntity("&UpArrowBar;", "\u2912");
            RegisterEntity("&UpArrowDownArrow;", "\u21C5");
            RegisterEntity("&UpDownArrow;", "\u2195");
            RegisterEntity("&Updownarrow;", "\u21D5");
            RegisterEntity("&updownarrow;", "\u2195");
            RegisterEntity("&UpEquilibrium;", "\u296E");
            RegisterEntity("&upharpoonleft;", "\u21BF");
            RegisterEntity("&upharpoonright;", "\u21BE");
            RegisterEntity("&uplus;", "\u228E");
            RegisterEntity("&UpperLeftArrow;", "\u2196");
            RegisterEntity("&UpperRightArrow;", "\u2197");
            RegisterEntity("&Upsi;", "\u03D2");
            RegisterEntity("&upsi;", "\u03C5");
            RegisterEntity("&upsih;", "\u03D2");
            RegisterEntity("&Upsilon;", "\u03A5");
            RegisterEntity("&upsilon;", "\u03C5");
            RegisterEntity("&UpTee;", "\u22A5");
            RegisterEntity("&UpTeeArrow;", "\u21A5");
            RegisterEntity("&upuparrows;", "\u21C8");
            RegisterEntity("&urcorn;", "\u231D");
            RegisterEntity("&urcorner;", "\u231D");
            RegisterEntity("&urcrop;", "\u230E");
            RegisterEntity("&Uring;", "\u016E");
            RegisterEntity("&uring;", "\u016F");
            RegisterEntity("&urtri;", "\u25F9");
            RegisterEntity("&Uscr;", "\uD835\uDCB0");
            RegisterEntity("&uscr;", "\uD835\uDCCA");
            RegisterEntity("&utdot;", "\u22F0");
            RegisterEntity("&Utilde;", "\u0168");
            RegisterEntity("&utilde;", "\u0169");
            RegisterEntity("&utri;", "\u25B5");
            RegisterEntity("&utrif;", "\u25B4");
            RegisterEntity("&uuarr;", "\u21C8");
            RegisterEntity("&Uuml;", "\u00DC");
            RegisterEntity("&uuml;", "\u00FC");
            RegisterEntity("&uwangle;", "\u29A7");
            RegisterEntity("&vangrt;", "\u299C");
            RegisterEntity("&varepsilon;", "\u03F5");
            RegisterEntity("&varkappa;", "\u03F0");
            RegisterEntity("&varnothing;", "\u2205");
            RegisterEntity("&varphi;", "\u03D5");
            RegisterEntity("&varpi;", "\u03D6");
            RegisterEntity("&varpropto;", "\u221D");
            RegisterEntity("&vArr;", "\u21D5");
            RegisterEntity("&varr;", "\u2195");
            RegisterEntity("&varrho;", "\u03F1");
            RegisterEntity("&varsigma;", "\u03C2");
            RegisterEntity("&varsubsetneq;",  "\u228A\uFE00");
            RegisterEntity("&varsubsetneqq;",  "\u2ACB\uFE00");
            RegisterEntity("&varsupsetneq;",  "\u228B\uFE00");
            RegisterEntity("&varsupsetneqq;",  "\u2ACC\uFE00");
            RegisterEntity("&vartheta;", "\u03D1");
            RegisterEntity("&vartriangleleft;", "\u22B2");
            RegisterEntity("&vartriangleright;", "\u22B3");
            RegisterEntity("&Vbar;", "\u2AEB");
            RegisterEntity("&vBar;", "\u2AE8");
            RegisterEntity("&vBarv;", "\u2AE9");
            RegisterEntity("&Vcy;", "\u0412");
            RegisterEntity("&vcy;", "\u0432");
            RegisterEntity("&VDash;", "\u22AB");
            RegisterEntity("&Vdash;", "\u22A9");
            RegisterEntity("&vDash;", "\u22A8");
            RegisterEntity("&vdash;", "\u22A2");
            RegisterEntity("&Vdashl;", "\u2AE6");
            RegisterEntity("&Vee;", "\u22C1");
            RegisterEntity("&vee;", "\u2228");
            RegisterEntity("&veebar;", "\u22BB");
            RegisterEntity("&veeeq;", "\u225A");
            RegisterEntity("&vellip;", "\u22EE");
            RegisterEntity("&Verbar;", "\u2016");
            RegisterEntity("&verbar;", "\u007C");
            RegisterEntity("&Vert;", "\u2016");
            RegisterEntity("&vert;", "\u007C");
            RegisterEntity("&VerticalBar;", "\u2223");
            RegisterEntity("&VerticalLine;", "\u007C");
            RegisterEntity("&VerticalSeparator;", "\u2758");
            RegisterEntity("&VerticalTilde;", "\u2240");
            RegisterEntity("&VeryThinSpace;", "\u200A");
            RegisterEntity("&Vfr;", "\uD835\uDD19");
            RegisterEntity("&vfr;", "\uD835\uDD33");
            RegisterEntity("&vltri;", "\u22B2");
            RegisterEntity("&vnsub;",  "\u2282\u20D2");
            RegisterEntity("&vnsup;",  "\u2283\u20D2");
            RegisterEntity("&Vopf;", "\uD835\uDD4D");
            RegisterEntity("&vopf;", "\uD835\uDD67");
            RegisterEntity("&vprop;", "\u221D");
            RegisterEntity("&vrtri;", "\u22B3");
            RegisterEntity("&Vscr;", "\uD835\uDCB1");
            RegisterEntity("&vscr;", "\uD835\uDCCB");
            RegisterEntity("&vsubnE;",  "\u2ACB\uFE00");
            RegisterEntity("&vsubne;",  "\u228A\uFE00");
            RegisterEntity("&vsupnE;",  "\u2ACC\uFE00");
            RegisterEntity("&vsupne;",  "\u228B\uFE00");
            RegisterEntity("&Vvdash;", "\u22AA");
            RegisterEntity("&vzigzag;", "\u299A");
            RegisterEntity("&Wcirc;", "\u0174");
            RegisterEntity("&wcirc;", "\u0175");
            RegisterEntity("&wedbar;", "\u2A5F");
            RegisterEntity("&Wedge;", "\u22C0");
            RegisterEntity("&wedge;", "\u2227");
            RegisterEntity("&wedgeq;", "\u2259");
            RegisterEntity("&weierp;", "\u2118");
            RegisterEntity("&Wfr;", "\uD835\uDD1A");
            RegisterEntity("&wfr;", "\uD835\uDD34");
            RegisterEntity("&Wopf;", "\uD835\uDD4E");
            RegisterEntity("&wopf;", "\uD835\uDD68");
            RegisterEntity("&wp;", "\u2118");
            RegisterEntity("&wr;", "\u2240");
            RegisterEntity("&wreath;", "\u2240");
            RegisterEntity("&Wscr;", "\uD835\uDCB2");
            RegisterEntity("&wscr;", "\uD835\uDCCC");
            RegisterEntity("&xcap;", "\u22C2");
            RegisterEntity("&xcirc;", "\u25EF");
            RegisterEntity("&xcup;", "\u22C3");
            RegisterEntity("&xdtri;", "\u25BD");
            RegisterEntity("&Xfr;", "\uD835\uDD1B");
            RegisterEntity("&xfr;", "\uD835\uDD35");
            RegisterEntity("&xhArr;", "\u27FA");
            RegisterEntity("&xharr;", "\u27F7");
            RegisterEntity("&Xi;", "\u039E");
            RegisterEntity("&xi;", "\u03BE");
            RegisterEntity("&xlArr;", "\u27F8");
            RegisterEntity("&xlarr;", "\u27F5");
            RegisterEntity("&xmap;", "\u27FC");
            RegisterEntity("&xnis;", "\u22FB");
            RegisterEntity("&xodot;", "\u2A00");
            RegisterEntity("&Xopf;", "\uD835\uDD4F");
            RegisterEntity("&xopf;", "\uD835\uDD69");
            RegisterEntity("&xoplus;", "\u2A01");
            RegisterEntity("&xotime;", "\u2A02");
            RegisterEntity("&xrArr;", "\u27F9");
            RegisterEntity("&xrarr;", "\u27F6");
            RegisterEntity("&Xscr;", "\uD835\uDCB3");
            RegisterEntity("&xscr;", "\uD835\uDCCD");
            RegisterEntity("&xsqcup;", "\u2A06");
            RegisterEntity("&xuplus;", "\u2A04");
            RegisterEntity("&xutri;", "\u25B3");
            RegisterEntity("&xvee;", "\u22C1");
            RegisterEntity("&xwedge;", "\u22C0");
            RegisterEntity("&Yacute;", "\u00DD");
            RegisterEntity("&yacute;", "\u00FD");
            RegisterEntity("&YAcy;", "\u042F");
            RegisterEntity("&yacy;", "\u044F");
            RegisterEntity("&Ycirc;", "\u0176");
            RegisterEntity("&ycirc;", "\u0177");
            RegisterEntity("&Ycy;", "\u042B");
            RegisterEntity("&ycy;", "\u044B");
            RegisterEntity("&yen;", "\u00A5");
            RegisterEntity("&Yfr;", "\uD835\uDD1C");
            RegisterEntity("&yfr;", "\uD835\uDD36");
            RegisterEntity("&YIcy;", "\u0407");
            RegisterEntity("&yicy;", "\u0457");
            RegisterEntity("&Yopf;", "\uD835\uDD50");
            RegisterEntity("&yopf;", "\uD835\uDD6A");
            RegisterEntity("&Yscr;", "\uD835\uDCB4");
            RegisterEntity("&yscr;", "\uD835\uDCCE");
            RegisterEntity("&YUcy;", "\u042E");
            RegisterEntity("&yucy;", "\u044E");
            RegisterEntity("&Yuml;", "\u0178");
            RegisterEntity("&yuml;", "\u00FF");
            RegisterEntity("&Zacute;", "\u0179");
            RegisterEntity("&zacute;", "\u017A");
            RegisterEntity("&Zcaron;", "\u017D");
            RegisterEntity("&zcaron;", "\u017E");
            RegisterEntity("&Zcy;", "\u0417");
            RegisterEntity("&zcy;", "\u0437");
            RegisterEntity("&Zdot;", "\u017B");
            RegisterEntity("&zdot;", "\u017C");
            RegisterEntity("&zeetrf;", "\u2128");
            RegisterEntity("&ZeroWidthSpace;", "\u200B");
            RegisterEntity("&Zeta;", "\u0396");
            RegisterEntity("&zeta;", "\u03B6");
            RegisterEntity("&Zfr;", "\u2128");
            RegisterEntity("&zfr;", "\uD835\uDD37");
            RegisterEntity("&ZHcy;", "\u0416");
            RegisterEntity("&zhcy;", "\u0436");
            RegisterEntity("&zigrarr;", "\u21DD");
            RegisterEntity("&Zopf;", "\u2124");
            RegisterEntity("&zopf;", "\uD835\uDD6B");
            RegisterEntity("&Zscr;", "\uD835\uDCB5");
            RegisterEntity("&zscr;", "\uD835\uDCCF");
            RegisterEntity("&zwj;", "\u200D");
            RegisterEntity("&zwnj;", "\u200C");
        }

        /// <summary>
        /// Create a new entity
        /// </summary>
        private HEntity(String name, Char[] characters)
        {
            this.Name = name;
            this.Characters = characters;
        }

        #region Entities management

        /// <summary>
        /// List all registered entities
        /// </summary>
        public static IEnumerable<HEntity> GetEntities()
        {
            return EntityNames.Values;
        }

        /// <summary>
        /// Find an entity by the name
        /// </summary>
        /// <remarks>
        /// The name is case sensitive.
        /// </remarks>
        public static HEntity FindEntityByName(String name)
        {
            if (String.IsNullOrWhiteSpace(name)) return null;
            HEntity result;
            if (EntityNames.TryGetValue(name, out result))
                return result;
            return null;
        }

        /// <summary>
        /// Find an entity by the characters list.
        /// </summary>
        /// <remarks>
        /// The search by <paramref name="characters"/> is strict.
        /// </remarks>
        /// <param name="characters">List of the characters in a string</param>
        /// <returns>The entity or null if not found.</returns>
        public static HEntity FindEntityByChars(String characters)
        {
            if (String.IsNullOrEmpty(characters)) return null;
            return FindEntityByChars(characters.ToCharArray());
        }

        /// <summary>
        /// Find an entity by the characters list.
        /// </summary>
        /// <remarks>
        /// The search by <paramref name="characters"/> is strict.
        /// </remarks>
        /// <param name="characters">List of the characters.</param>
        /// <returns>The entity or null if not found.</returns>
        public static HEntity FindEntityByChars(params char[] characters)
        {
            if (characters == null || characters.Length == 0) return null;
            // If one characters search on the Sinlge index
            if (characters.Length == 1)
            {
                HEntity result = null;
                if (EntitySingleIndex.TryGetValue(characters[0], out result))
                    return result;
                return null;
            }
            // Find the chain
            EntityIndex idx = null;
            if (!EntityIndexes.TryGetValue(characters[0], out idx))
                return null;
            // Browse all the chain
            for (int i = 1, ln = characters.Length; i < ln; i++)
            {
                if (idx.Entities == null) return null;
                var c = characters[i];
                idx = idx.Entities.FirstOrDefault(ie => ie.Index == c);
                if (idx == null) break;
            }
            // Returns the entity in the index
            return idx == null ? null : idx.Entity;
        }

        /// <summary>
        /// Decode all entities in a text.
        /// </summary>
        public static String HtmlDecode(String value, bool removeUnknownOrInvalidEntities = false)
        {
            // If nothing to decode returns
            if (String.IsNullOrEmpty(value)) return value;
            // Prepare result
            StringBuilder result = new StringBuilder();
            // Parse
            int pos = 0, valLen = value.Length;
            // 0: Search entity
            // 1: In entity
            int state = 0;
            int sPos = pos;
            while (pos < valLen)
            {
                var c = value[pos];
                switch (state)
                {
                    // Search entity
                    case 0:
                        if (c == '&')
                        {
                            if (sPos < pos)
                                result.Append(value.Substring(sPos, pos - sPos));
                            sPos = pos;
                            state = 1;
                        }
                        break;
                    // In entity
                    case 1:
                        switch (c)
                        {
                            // End of entity
                            case ';':
                                // Name if entity
                                String name = value.Substring(sPos + 1, pos - sPos - 1);
                                // Numeric entity
                                if (name.StartsWith("#"))
                                {
                                    try
                                    {
                                        string codeStr = name.Substring(1).Trim().ToLower();
                                        int fromBase;
                                        if (codeStr.StartsWith("x"))
                                        {
                                            fromBase = 16;
                                            codeStr = codeStr.Substring(1);
                                        }
                                        else
                                        {
                                            fromBase = 10;
                                        }
                                        int code = Convert.ToInt32(codeStr, fromBase);
                                        result.Append(Convert.ToChar(code));
                                    }
                                    catch
                                    {
                                        if (!removeUnknownOrInvalidEntities)
                                            result.Append("&").Append(name).Append(";");
                                    }
                                }
                                else
                                {
                                    var ent = FindEntityByName(name);
                                    if (ent != null)
                                        result.Append(ent.ToString());
                                    else if (!removeUnknownOrInvalidEntities)
                                        result.Append("&").Append(name).Append(";");
                                }
                                sPos = pos + 1;
                                state = 0;
                                break;

                            // New entity
                            case '&':
                                // Current entity is not closed
                                result.Append(value.Substring(sPos, pos - sPos));
                                sPos = pos;
                                break;

                            //// Another char
                            //default:
                            //    break;
                        }
                        break;
                }
                // Next char
                pos++;
            }
            // Add the rest
            if (sPos < pos)
            {
                result.Append(value.Substring(sPos, pos - sPos));
            }
            // Returns the result
            return result.ToString();
        }

        /// <summary>
        /// Encode all chars to entites when required (char &gt; 127).
        /// </summary>
        public static String HtmlEncode(String value)
        {
            // If nothing to encode returns
            if (String.IsNullOrEmpty(value)) return value;
            // Prepare result
            StringBuilder result = new StringBuilder();
            // Parse
            int pos = 0, valLen = value.Length;
            int sPos = pos;
            while (pos < valLen)
            {
                Char c = value[pos];
                if (c > 127 || c == '&' || c == '<' || c == '>' || c == '"')
                {
                    if (sPos < pos)
                        result.Append(value.Substring(sPos, pos - sPos));
                    // Search from index
                    EntityIndex index;
                    HEntity entity = null;
                    if (EntityIndexes.TryGetValue(c, out index))
                    {
                        int ci = pos + 1;
                        while (index != null && index.Entities != null && ci < valLen)
                            index = index.Entities.FirstOrDefault(ei => ei.Index == value[ci++]);
                        if (index != null && index.Entity != null)
                        {
                            pos = ci - 1;
                            entity = index.Entity;
                        }
                    }
                    if (entity == null)
                        EntitySingleIndex.TryGetValue(c, out entity);
                    // If the entity is found use name
                    if (entity != null)
                        result.Append("&").Append(entity.Name).Append(";");
                    else
                        result.Append("&#x").Append(((int)c).ToString("x4")).Append(";");

                    sPos = pos + 1;
                }
                pos++;
            }
            if (sPos < pos)
                result.Append(value.Substring(sPos, pos - sPos));
            // Returns result
            return result.ToString();
        }

        #endregion

        /// <summary>
        /// Convert to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Characters.Length == 1) return Characters[0].ToString();
            StringBuilder sb = new StringBuilder();
            foreach (var c in Characters)
                sb.Append(c);
            return sb.ToString();
        }

        /// <summary>
        /// Name
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// List of unicode chars
        /// </summary>
        public Char[] Characters { get; private set; }

    }
}
