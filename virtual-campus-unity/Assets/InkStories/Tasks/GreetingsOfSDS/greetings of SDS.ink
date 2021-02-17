#after: greetings of registry
// mission name: greetings of SDS 

#name: 数据科学院的问候
#description: 院长们在新生入学期间跑到了学校各处，受教务处老师之托，你需要找到他们。根据教务处老师的信息，数据科学院院长可能在<color=red>乐天楼</color>一楼食堂。

=== func_start ===
#collidetrigger: SDS_dean_tmp
//SDS院长此时应该在乐天一食堂门口，完成任务后，常驻位置为道远楼下
你好，我是数据科学院的院长戴建岗。
*你好，您就是数据科学院院长吗
啊呀，同学你好啊。是的是的，我是院长戴建岗。
-
*院长好，教务处老师好像在找你呢
诶呀，出来逛太久了，是该回去了。今天是新同学入学的日子。
-
+n
话说，你也是新同学吗？
-
*是的
那你对数据科学院有所了解吗，想听听一些有关数据科学院内部各个专业的简介吗？
 **想
 好的->introduction
 **不想
 哦？你可要想好了，如果你对数据科学院有兴趣的话，跳过这些介绍，可能也会错过一些有用的<color=blue>信息</color>哦。
    ***确认跳过
    真可惜，那我先回去了。如果以后想找我的话，就去<color=red>涂辉龙楼</color>下找我。
        ****好的
        #enableNPC: SDS_dean
        #upd_description:你已经在<color=red>乐天楼</color>食堂附近找到了<color=red>数据科学学院院长</color>了。现在该去找剩下的院长了！
        #endstory
        ->END
    
=== ending ===
+n
那么，这就是数据科学学院的所有专业了。
-
+n
啊……也该回去了，新生入学的日子，我也得去看看。
-
+n
如果以后想找我的话，就去<color=red>涂辉龙楼</color>下找我。
-
*嗯嗯
#enableNPC: SDS_dean
#upd_description:你已经在<color=red>乐天楼</color>食堂附近找到了<color=red>数据科学学院院长</color>了。现在该去找剩下的院长了！
#endstory
->END

=== introduction ===
VAR flag = true
那么，你想了解？
*本科生 -> undergraduate
*研究生 -> graduate

//本科生部分------------------------------------
== undergraduate ==
*数据科学和大数据技术 ->undergraduate.BD
*计算机科学与技术 ->undergraduate.CS
*统计学 ->undergraduate.STA
*金融工程（与经管学院联合创办）->undergraduate.FE
* ->
    ~ flag = !flag
    {flag == false:
    ++n 
    那么你目前已经听完了数据科学学院<color=blue>本科生</color>所有专业的简介了。你还想要听一下<color=blue>研究生</color>的吗？（一旦要听，就要把所有研究生专业全部听完）
        +++是
        ->graduate
        +++不了，谢谢
        ->ending
    - else:
    ++n
    ->ending
    }
    
= BD
数据科学与大数据技术是一门跨学科专业，涵盖了从数据收集、分析到决策的全部数据分析要素。数据科学与大数据技术本科课程的目标，是培养具备数据科学所需的扎实数学和统计基础、理解和规划数据分析问题的计算和分析思维，以及对现实数据分析问题实现可扩展性解决方案的实际操作能力的数据科学领域人才。
-
+n
首先，我们有着非常强大的师资力量：数据科学与大数据技术专业的教授队伍由海内外知名学者组成，所有教授均拥有世界一流学府的博士学位，在学术界和工业界相关领域具有显著的国际影响力。我们的老师不仅研究成果斐然，屡获国际奖项，而且教学经验丰富，培养出了许多优秀的年轻才俊。
-
+n
其次，数据科学与大数据技术专业的学科特色在于理论与应用的结合。我们专业设有一个高级数据科学领域与四个应用领域，分别是运筹管理领域、生命科学领域、计算领域与金融经济领域。学生在掌握学科基础知识的同时，还可以按照自身的兴趣爱好选择一个或多个领域，学习数据科学在各领域中的实际应用，这种教育模式不仅可以拓宽学生们的知识面，也可以提高学生升学和就业的竞争力。
-
+n
本课程旨在培养学生具备扎实理论、算法基础和实际操作能力，并能够解决现实世界中数据科学与大数据相关的数据分析问题。
-
+n
完成本课程的学生将具备：
数据分析与机器学习的理论和方法论基础；
实现数据分析和机器学习算法的实际操作技术；
解决实际应用中常见的数据分析问题的能力，并能将计算结果应用于下游决策问题。 
应用范围与就业
-
+n
大数据技术的应用范围非常广泛，常用于金融、互联网、医疗、物流、教育等行业。互联网的迅速发展，使得大数据相关产业市场规模不断扩大，人才缺口也日益增大。学生可以根据自身兴趣选择就业方向，也可前往国内外知名高校继续深造。
-
->undergraduate

= CS
+n
计算机科学与技术是信息化的核心，并且属涵盖面广、应用面宽、应用层次跨度大的学科专业之一。中国的经济环境在过去的十多年中发生了显著的变化，信息和通信技术已成为中国经济发展的主要动力。
-
+n
香港中文大学（深圳）致力建立成为世界级的教育和科研重镇，并决心在计算机科学与技术领域展开国际前沿的研究。为了实现这一目标，我们将会全球招聘一流学者加盟，精编学科课程，并配置先进的教学科研设施。
-
+n
本专业的毕业生将会在数学、科学和商学等学科上建立扎实的基础，并会掌握许多广泛应用的知识和技能，使能在所处领域内从事前沿工作。同时，他们所接受的跨学科教育，将会令其在金融、工业、政府机构，以及在学术领域进一步的深造学习中出类拔萃。
-
->undergraduate

= EE
+n
随着科技的发展以及⼈类对于能源的进⼀步开发，如何促进能源清洁⾼效利⽤，提升能源综合效率，推动能源市场开放和产业升级成为能源领域的新的关注点。同时，互联互通的科技正在改变社会的运⾏⽅式，学术界提出要“构建⼀个更有智慧的地球”，投射到能源领域，产⽣了“智慧能源”这⼀前沿理念，将互联⽹与能源⽣产、传输、存储、消费以及能源市场深度融合。
-
+n
本专业为跨学科专业，旨在探索“智慧能源系统”这⼀新兴能源发展新形态，结合了物理与材料科学的传统训练，以及⽣物分⼦⼯程、光化学及电化学的现代元素。除新能源课程外，还包括基本电路原理、信号系统以及⾼级统计学课程，将新能源学习与数据计算技术结合，让学⽣能够平衡智慧能源系统所需的能源与互联⽹技术⽅⾯的训练，同时适应⽬前就业市场对能源⽅向复合型⼈才的需求。
-
+n
学院现正积极与著名海外⾼校合作，设计3+1+1新能源本硕联读课程，本专业亦设有毕业项⽬(Final Year Project)，训练同学独⽴动⼿和研究能⼒，对未来升学或就业有极⼤帮助，本专业毕业⽣将能进⼊包括“智能电⽹”等新兴智慧能源系统领域，以及⾼等院校和科研院所从事能源研究。
-
+n
“智慧能源系统”是国内⼤⼒推进“互联⽹+”智慧能源发展的主要体现，具有⼴阔的前景，急需互联⽹及能源⽅⾯的跨学科技能科学家及⼯程师。
-

->undergraduate

= MAT
+n
数学知识对所有科学与工程人才至关重要，它是制定理论与自然法则及表达理念的语言。

本专业旨在使学生具备扎实的基础及应用数学的能力。专业课程不仅教授数学基本理论，还将训练学生在商业、金融、政府、工程、物理、生命科学及社会科学领域内应用数学模型和计算方法解决实际问题。
-
+n
在本专业课程的前两年，学生将学习数学和一般理工科的基础科目。在第三年和第四年，学生将会学习选定的专修方向。本专业也将为拟开阔视野或在数学各专业范围内深造的学生提供若干关于数学最新领域的高级课程。
-
+n
数学及应用数学专业提供三个专修方向给学生修读：理论数学/应用数学/金融数学
-
+n
虽然本专业在初期将偏重数学的实际应用，但也希望为从事纯数学工作的学生提供数学理论专业范围的课程。此外，本专业还包含了日趋重要的金融数学专业范围以满足对此专业人才的大量需求。
-

->undergraduate

= STA
+n
统计学研究数据的收集、分析和诠释，从而获取信息和支持决策。统计学已广泛应用于各领域，包括从生物学、医药、工程到心理学、会计、金融和政府决策。随着传感、通信与计算技术的发展，数据的获取比以往任何时候都更为方便。因此，在很多领域都出现了各种各样的海量数据。
-
+n
通过适当的建模和分析，大数据可以有效地支持推断、估计、跟踪、学习和决策。然而，大数据的规模和复杂性带来了新的挑战和问题。因此，在统计理论和计算技术接受过严格教育的毕业生，将会在处理各种统计问题，尤其是大数据问题上，发挥越来越重要的作用。他们将会在就业市场中供不应求，并且将会对经济发展起着不可估量的作用。
-
+n
为了满足信息时代对高素质统计人才日益增长的需求，本专业课程涵盖了统计学各个核心领域，学生将会掌握统计学的基本理论和解决实际问题所需的技能。课程主要包括：统计学基础，计算和数据管理，统计学理论和方法，统计学在商业、金融、医药、精算、质量控制和风险管理科学等的应用。
-
+n

统计学专业提供三个专修方向给学生修读：数据科学/金融统计/统计科学。
-
+n
本专业的毕业生将能胜任于工商业和政府机构的工作，以及在科学、工程、工商管理等领域进一步深造和从事研究。
-
->undergraduate

= FE
+n
本课程的毕业生具有较强的计量及分析能力，有成为未来金融行业领袖的潜质，并能满足金融行业对人才的需求。
-
+n
毕业生将具备全面的金融建模与预测能力，因此能胜任对计量及分析能力要求极高的职位与工作，例如估值、投资组合分析、资产配置、信用分析、风险模型，以及结构性融资。本课程为毕业生在投资银行、商业与企业银行，以及金融服务单位中的职业发展打下良好基础。
-
+n
本课程提供以下两个专业方向：量化金融/金融科技
-

->undergraduate


// 研究生部分-----------------------------------
== graduate ==
*金融工程 ->graduate.FE
*数据科学 ->graduate.DS
*->
    ~ flag = !flag
    {flag == false:
    ++n 
    那么你目前已经听完了数据科学学院<color=blue>研究生</color>所有专业的简介了。你还想要听一下<color=blue>本科生</color>的吗？（一旦要听，就要把所有本科生专业全部听完）
        +++是
        ->undergraduate
        +++不了，谢谢
        ->ending
    - else:
    ++n
    ->ending
    }
    
= FE
香港中文大学（深圳）与香港中文大学于2015年合作开办金融工程理学硕士项目。该项目是香港中文大学（深圳）设立的首批硕士课程之一。金融工程这一学科所涉及的知识是多方面的，它融合了金融理论、高等数学、建模、分析与优化，程序设计及其实现，产品开发等方面的知识和技术。
+n
香港中文大学金融工程硕士项目旨在向学生提供全面的金融理论知识，特别是定量分析方法在金融中的应用。本项目为立志投身于证券，银行，金融管理，管理与策略咨询，定量分析等行业的人员设立。该项目致力于帮助学生掌握金融理论，定量分析方法以及程序设计技术，使之在面临迅速发展的中国以及国际金融市场所带来的挑战时，能够发挥所学，服务企业与社会。
-
->graduate

= DS
数据科学理学硕士项目由香港中文大学（深圳）数据科学学院与经管学院共同开设，并与深圳市大数据研究院和深圳高等金融研究院合作。
+n
项目旨在使学生掌握大数据及商业分析方面的基础理论知识和专业技能，接触到支撑大数据技术的前沿理论与方法，并能在商业、政府、安全、医疗、生物、自然科学、环境等领域中充分利用所学知识解决与大数据采集、管理及分析相关的问题。
-
->graduate
