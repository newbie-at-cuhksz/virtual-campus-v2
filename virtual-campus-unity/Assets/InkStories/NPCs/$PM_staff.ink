=== func_1 ===
清洁校园，养护花草，修缮设施，失物招领...物业处为您服务。
    *\*离开*
        ->DONE
    *物业介绍
        ->introduction

=== func_2 ===
#require: blackswan
{~清洁校园，养护花草，修缮设施，失物招领...物业处为您服务。|诶？黑天鹅不存在吗？说起来，我身边也确实没有人真的见过啊，大家都以为是物业里面的其他同事去抓的。}
    *\*离开* 
        ->DONE
    *物业介绍
        ->introduction

=== func_3 ===
#require: power cut
{~清洁校园，养护花草，修缮设施，失物招领...物业处为您服务。|诶？黑天鹅不存在吗？说起来，我身边也确实没有人真的见过啊，大家都以为是物业里面的其他同事去抓的。|上次停电事故，真的是麻烦你了。我们物业一定会注意，不再发生类似事故的。}
    *\*离开* 
        ->DONE
    *物业介绍
        ->introduction

=== func_4 ===
#reuiqre: blackswan
#require: photography(I)
{~清洁校园，养护花草，修缮设施，失物招领...物业处为您服务。|诶？黑天鹅不存在吗？说起来，我身边也确实没有人真的见过啊，大家都以为是物业里面的其他同事去抓的。|上次停电事故，真的是麻烦你了。我们物业一定会注意，不再发生类似事故的。|我在教摄影的时候，有些不一样？自然啊，爱好是爱好，工作是工作，还是要分得清才行。}
    *\*离开* 
        ->DONE
    *物业介绍
        ->introduction

=== introduction ===
中航物业是中国物业管理行业首批国家一级资质企业，聚焦高校物业解决方案，为高校提供更加全面、完善和优质的服务，多年来发展和积累了丰富校园服务经验，位列中国校园物业服务实体（企业）百强前列。
-
+n
中航物业自香港中文大学（深圳）校区 2014 年建校办学之初便入驻服务，与学校共同成长，目前服务建筑面积 38 万㎡，服务师生 4500 人。秉承香港中文大学（深圳）大学“结合传统与现代，融会中国与西方”的教育理念，注重“全人教育”与“书院制”传统，以及香港中文大学（深圳）徐扬生校长对大学的定位：
-
+n
“我们坚信大学不但应该为学生传授专业知识，更应该引导学生全面发展，追求智慧，探索人生、社会和世界的意义，鼓励学生成为“学贯中西”、“汇通古今”、“融合文理”，富有社会责任感的新时代公民”，竭诚为全校师生提供物业服务。
-
+n
    ->DONE